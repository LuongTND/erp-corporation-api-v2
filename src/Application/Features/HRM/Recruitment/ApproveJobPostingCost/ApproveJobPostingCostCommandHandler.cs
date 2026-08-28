namespace Application;

public sealed class ApproveJobPostingCostCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<ApproveJobPostingCostCommand, Unit>
{
    public async Task<Unit> Handle(ApproveJobPostingCostCommand cmd, CancellationToken ct)
    {
        var posting = await unitOfWork.Repository<JobPosting>()
            .FindAsync(p => p.Id == cmd.PostingId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobPosting", cmd.PostingId));

        if (posting.CostStatus != JobPostingCostStatus.PendingApproval)
            throw new BadRequestException("Chi phí không ở trạng thái chờ duyệt.");

        posting.CostStatus = JobPostingCostStatus.Approved;
        posting.CostApprovedByUserId = currentUser.UserId;
        posting.CostApprovedAt = DateTimeOffset.UtcNow;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
