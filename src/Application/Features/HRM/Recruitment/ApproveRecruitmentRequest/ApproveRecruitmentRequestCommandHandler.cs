namespace Application;

public sealed class ApproveRecruitmentRequestCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ApproveRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(ApproveRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status != RecruitmentRequestStatus.PendingApproval)
            throw new BadRequestException("Chỉ có thể duyệt phiếu ở trạng thái PendingApproval.");

        request.Status = RecruitmentRequestStatus.Approved;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
