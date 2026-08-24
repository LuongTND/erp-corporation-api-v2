namespace Application;

public sealed class DeleteRecruitmentApproverCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRecruitmentApproverCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRecruitmentApproverCommand cmd, CancellationToken ct)
    {
        var config = await unitOfWork.Repository<RecruitmentApproverConfig>()
            .FindAsync(c => c.Id == cmd.ConfigId, ct)
            ?? throw new NotFoundException($"Cấu hình người duyệt {cmd.ConfigId} không tồn tại");

        await unitOfWork.Repository<RecruitmentApproverConfig>().RemoveAsync(config);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
