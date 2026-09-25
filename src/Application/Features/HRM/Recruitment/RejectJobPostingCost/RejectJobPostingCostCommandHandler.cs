namespace Application;

public sealed class RejectJobPostingCostCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RejectJobPostingCostCommand, Unit>
{
    public async Task<Unit> Handle(RejectJobPostingCostCommand cmd, CancellationToken ct)
    {
        var posting = await unitOfWork.Repository<JobPosting>()
            .FindAsync(p => p.Id == cmd.PostingId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobPosting", cmd.PostingId));

        if (posting.CostStatus != JobPostingCostStatus.PendingApproval)
            throw new BadRequestException("Chi phí không ở trạng thái chờ duyệt.");

        posting.CostStatus = JobPostingCostStatus.Rejected;
        posting.CostRejectionNote = cmd.RejectionNote;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
