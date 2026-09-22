namespace Application;

public sealed class UpdateJobPostingCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobPostingCommand, Unit>
{
    public async Task<Unit> Handle(UpdateJobPostingCommand cmd, CancellationToken ct)
    {
        var posting = await unitOfWork.Repository<Domain.JobPosting>()
            .FindTrackedAsync(p => p.Id == cmd.PostingId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobPosting", cmd.PostingId));

        if (cmd.AssignedToUserId.HasValue && cmd.AssignedToUserId != posting.AssignedToUserId)
            _ = await unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == cmd.AssignedToUserId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.AssignedToUserId.Value));

        posting.Channel = cmd.Channel;
        posting.Title = cmd.Title;
        posting.WorkLocation = cmd.WorkLocation;
        posting.PostUrl = cmd.PostUrl;
        posting.Requirements = cmd.Requirements;
        posting.AssignedToUserId = cmd.AssignedToUserId;
        posting.ExpiresAt = cmd.ExpiresAt;

        if (cmd.Status == JobPostingStatus.Published && posting.Status != JobPostingStatus.Published)
            posting.PostedAt = DateTimeOffset.UtcNow;

        posting.Status = cmd.Status;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
