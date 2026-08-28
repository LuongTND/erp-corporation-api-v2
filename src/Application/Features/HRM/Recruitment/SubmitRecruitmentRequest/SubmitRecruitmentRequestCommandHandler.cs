namespace Application;

public sealed class SubmitRecruitmentRequestCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(SubmitRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status != RecruitmentRequestStatus.Draft &&
            request.Status != RecruitmentRequestStatus.NeedMoreInfo)
            throw new BadRequestException("Chỉ có thể gửi phiếu ở trạng thái Draft hoặc NeedMoreInfo.");

        request.Status = RecruitmentRequestStatus.PendingApproval;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
