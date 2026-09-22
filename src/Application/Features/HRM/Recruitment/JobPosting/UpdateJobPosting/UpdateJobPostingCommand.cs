namespace Application;

public sealed record UpdateJobPostingCommand(
    RecruitmentChannel Channel,
    string? Title,
    string? WorkLocation,
    string? PostUrl,
    string? Requirements,
    Guid? AssignedToUserId,
    JobPostingStatus Status,
    DateOnly? ExpiresAt
) : IRequest<Unit>
{
    public Guid PostingId { get; init; }
}
