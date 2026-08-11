namespace Application;

public sealed class UpdateUserStatusCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<UpdateUserStatusCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUserStatusCommand cmd, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>()
            .FindTrackedAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        var oldStatus = user.Status;

        if (oldStatus == cmd.NewStatus)
            throw new BadRequestException($"Nhân sự đã ở trạng thái {cmd.NewStatus}.");

        user.ChangeStatus(cmd.NewStatus);

        var now = DateTimeOffset.UtcNow;

        await unitOfWork.Repository<UserStatusHistory>().AddAsync(new UserStatusHistory
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            OldStatus = oldStatus,
            NewStatus = cmd.NewStatus,
            Note = cmd.Note,
            ChangedBy = currentUser.UserId,
            ChangedAt = now,
        });

        await unitOfWork.Repository<WorkHistory>().AddAsync(new WorkHistory
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            ChangeType = WorkHistoryChangeType.Status,
            OldValue = oldStatus.ToString(),
            NewValue = cmd.NewStatus.ToString(),
            Note = cmd.Note,
            ChangedBy = currentUser.UserId,
            ChangedAt = now,
        });

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
