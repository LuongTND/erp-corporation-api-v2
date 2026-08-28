namespace Application;

public sealed class RequestMoreInfoRecruitmentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RequestMoreInfoRecruitmentCommand, Unit>
{
    public async Task<Unit> Handle(RequestMoreInfoRecruitmentCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status is not (RecruitmentRequestStatus.PendingLevel1Approval or RecruitmentRequestStatus.PendingLevel2Approval))
            throw new BadRequestException("Chỉ có thể yêu cầu thêm thông tin khi phiếu đang chờ duyệt.");

        request.Status = RecruitmentRequestStatus.NeedMoreInfo;
        request.NeedMoreInfoNote = cmd.NeedMoreInfoNote;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
