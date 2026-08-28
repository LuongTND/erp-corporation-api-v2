namespace Application;

public sealed class ApproveRecruitmentRequestCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<ApproveRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(ApproveRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status != RecruitmentRequestStatus.PendingLevel2Approval)
            throw new BadRequestException("Chỉ có thể duyệt cấp 2 khi phiếu đang ở trạng thái chờ duyệt cấp 2.");

        request.Status = RecruitmentRequestStatus.Approved;
        request.Level2ApproverId = userContext.UserId;
        request.Level2ApprovedAt = DateTimeOffset.UtcNow;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
