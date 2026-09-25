namespace Application;

public sealed record CreateJobPostingCommand(
    RecruitmentChannel Channel,
    string? Title,
    string? WorkLocation,
    string? PostUrl,
    string? Requirements,
    Guid? AssignedToUserId,
    DateOnly? ExpiresAt,
    JobPostingStatus Status = JobPostingStatus.Draft
) : IRequest<Guid>
{
    public Guid RecruitmentRequestId { get; init; }
}
