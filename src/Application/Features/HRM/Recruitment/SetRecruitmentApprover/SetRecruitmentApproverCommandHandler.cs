namespace Application;

public sealed class SetRecruitmentApproverCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SetRecruitmentApproverCommand, Guid>
{
    public async Task<Guid> Handle(SetRecruitmentApproverCommand cmd, CancellationToken ct)
    {
        var approverExists = await unitOfWork.Repository<User>().AnyAsync(u => u.Id == cmd.ApproverId, ct);
        if (!approverExists) throw new NotFoundException($"Người dùng {cmd.ApproverId} không tồn tại");

        // One approver per department (null DepartmentId = global default)
        var existing = await unitOfWork.Repository<RecruitmentApproverConfig>()
            .FindTrackedAsync(c => c.DepartmentId == cmd.DepartmentId, ct);

        if (existing is not null)
        {
            existing.ApproverId = cmd.ApproverId;
            existing.Note       = cmd.Note;
            await unitOfWork.EnsureSaveAsync(ct);
            return existing.Id;
        }

        var config = new RecruitmentApproverConfig
        {
            Id           = Guid.NewGuid(),
            ApproverId   = cmd.ApproverId,
            DepartmentId = cmd.DepartmentId,
            Note         = cmd.Note,
        };
        await unitOfWork.Repository<RecruitmentApproverConfig>().AddAsync(config);
        await unitOfWork.EnsureSaveAsync(ct);
        return config.Id;
    }
}
