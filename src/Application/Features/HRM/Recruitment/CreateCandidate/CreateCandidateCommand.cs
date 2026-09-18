namespace Application;

public sealed record CreateCandidateCommand(
    Guid? RecruitmentRequestId,
    string FullName,
    string? Email,
    string? Phone,
    string SourceChannel,
    string? Notes
) : IRequest<Guid>;
