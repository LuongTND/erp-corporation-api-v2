namespace Application;

public sealed class DeleteRecruitmentRequestCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status != RecruitmentRequestStatus.Draft)
            throw new BadRequestException("Chỉ có thể xoá phiếu ở trạng thái Draft.");

        request.IsDeleted = true;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
