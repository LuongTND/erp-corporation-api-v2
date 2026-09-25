namespace Application;

public sealed class DeleteJobPostingCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteJobPostingCommand, Unit>
{
    public async Task<Unit> Handle(DeleteJobPostingCommand cmd, CancellationToken ct)
    {
        var posting = await unitOfWork.Repository<Domain.JobPosting>()
            .FindTrackedAsync(p => p.Id == cmd.PostingId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobPosting", cmd.PostingId));

        if (posting.Applications.Count > 0)
            throw new BadRequestException("Không thể xoá tin tuyển dụng đã có hồ sơ ứng viên.");

        await unitOfWork.Repository<Domain.JobPosting>().RemoveAsync(posting);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
