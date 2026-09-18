namespace Application;

public sealed record UpdateCandidateCommand(
    Guid ApplicationId,
    string FullName,
    string? Email,
    string? Phone,
    string SourceChannel,
    string? Notes
) : IRequest<Unit>;
