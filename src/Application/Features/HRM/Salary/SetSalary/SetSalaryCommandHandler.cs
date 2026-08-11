namespace Application;

public sealed class SetSalaryCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<SetSalaryCommand, Guid>
{
    public async Task<Guid> Handle(SetSalaryCommand cmd, CancellationToken ct)
    {
        var userExists = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.Id == cmd.UserId && u.IsActive, ct);
        if (!userExists)
            throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        // Close record hiện tại (nếu có)
        var current = await unitOfWork.Repository<SalaryRecord>()
            .FindTrackedAsync(s => s.UserId == cmd.UserId && s.EffectiveTo == null, ct);

        if (current is not null)
        {
            if (current.EffectiveFrom >= cmd.EffectiveFrom)
                throw new BadRequestException("EffectiveFrom phải sau ngày hiệu lực của lương hiện tại.");

            current.EffectiveTo = cmd.EffectiveFrom.AddDays(-1);
        }

        var record = new SalaryRecord
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            HourlyRate = cmd.HourlyRate,
            EffectiveFrom = cmd.EffectiveFrom,
            Reason = cmd.Reason
        };

        await unitOfWork.Repository<SalaryRecord>().AddAsync(record);

        await unitOfWork.Repository<WorkHistory>().AddAsync(new WorkHistory
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            ChangeType = WorkHistoryChangeType.Salary,
            OldValue = current?.HourlyRate.ToString("F0"),
            NewValue = cmd.HourlyRate.ToString("F0"),
            Note = cmd.Reason,
            ChangedBy = currentUser.UserId,
            ChangedAt = DateTimeOffset.UtcNow,
        });

        await unitOfWork.EnsureSaveAsync(ct);
        return record.Id;
    }
}
