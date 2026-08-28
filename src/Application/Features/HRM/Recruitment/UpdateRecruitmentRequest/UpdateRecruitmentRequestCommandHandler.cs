namespace Application;

public sealed class UpdateRecruitmentRequestCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status is not (RecruitmentRequestStatus.Draft or RecruitmentRequestStatus.NeedMoreInfo))
            throw new BadRequestException("Chỉ có thể cập nhật phiếu ở trạng thái Draft hoặc NeedMoreInfo.");

        request.PositionTitle = cmd.PositionTitle;
        request.Headcount = cmd.Headcount;
        request.Reason = cmd.Reason;
        request.JobDescription = cmd.JobDescription;
        request.RequiredByDate = cmd.RequiredByDate;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
