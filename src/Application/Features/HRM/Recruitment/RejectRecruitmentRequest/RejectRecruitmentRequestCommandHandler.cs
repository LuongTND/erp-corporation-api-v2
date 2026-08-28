namespace Application;

public sealed class RejectRecruitmentRequestCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RejectRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(RejectRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status is not (RecruitmentRequestStatus.PendingLevel1Approval or RecruitmentRequestStatus.PendingLevel2Approval))
            throw new BadRequestException("Chỉ có thể từ chối phiếu đang chờ duyệt.");

        request.Status = RecruitmentRequestStatus.Rejected;
        request.RejectionNote = cmd.RejectionNote;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
