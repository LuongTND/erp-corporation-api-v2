namespace Application;

public sealed class TerminateContractCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<TerminateContractCommand>
{
    public async Task Handle(TerminateContractCommand cmd, CancellationToken ct)
    {
        var contract = await unitOfWork.Repository<EmploymentContract>()
            .FindTrackedAsync(c => c.Id == cmd.ContractId && c.UserId == cmd.UserId, ct);
        if (contract is null)
            throw new NotFoundException(ExceptionMessages.NotFound("EmploymentContract", cmd.ContractId));

        if (contract.Status != ContractStatus.Active)
            throw new BadRequestException("Chỉ có thể thanh lý hợp đồng đang hiệu lực.");

        var now = DateTimeOffset.UtcNow;

        contract.Status = ContractStatus.Terminated;
        contract.TerminationReason = cmd.Reason;
        contract.ModifiedAt = now;
        contract.UpdatedBy = currentUser.UserId;

        // Đồng bộ trạng thái user trong cùng transaction
        var user = await unitOfWork.Repository<User>()
            .FindTrackedAsync(u => u.Id == cmd.UserId, ct);
        if (user is not null && user.Status != UserStatus.Terminated)
        {
            var oldStatus = user.Status;
            user.ChangeStatus(UserStatus.Terminated);
            user.ModifiedAt = now;
            user.UpdatedBy = currentUser.UserId;

            await unitOfWork.Repository<WorkHistory>().AddAsync(new WorkHistory
            {
                Id = Guid.NewGuid(),
                UserId = cmd.UserId,
                ChangeType = WorkHistoryChangeType.Status,
                OldValue = oldStatus.ToString(),
                NewValue = UserStatus.Terminated.ToString(),
                Note = $"Tự động cập nhật khi chấm dứt HĐ: {cmd.Reason}",
                ChangedBy = currentUser.UserId,
                ChangedAt = now,
            });
        }

        await unitOfWork.EnsureSaveAsync(ct);
    }
}
