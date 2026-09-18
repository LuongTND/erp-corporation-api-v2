namespace Application;

public sealed class DeleteRecruitmentRequestCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<DeleteRecruitmentRequestCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindTrackedAsync(r => r.Id == cmd.RequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RequestId));

        if (request.Status != RecruitmentRequestStatus.Draft)
            throw new BadRequestException("Chỉ có thể xoá phiếu ở trạng thái Draft.");

        if (request.RequestedByUserId != userContext.UserId)
            throw new ForbiddenException("Chỉ người tạo phiếu mới có thể xoá.");

        request.IsDeleted = true;
        request.DeletedAt = DateTimeOffset.UtcNow;
        request.DeletedBy = userContext.UserId;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
