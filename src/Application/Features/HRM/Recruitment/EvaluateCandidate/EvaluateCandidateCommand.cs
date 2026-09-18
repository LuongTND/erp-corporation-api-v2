namespace Application;

public sealed record EvaluateCandidateCommand(
    Guid ApplicationId,
    int Score,
    string? StrengthNotes,
    string? WeaknessNotes,
    string Recommendation
) : IRequest<Guid>;
