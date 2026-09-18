namespace Application;

public sealed record EvaluateCandidateCommand(
    Guid CandidateId,
    bool IsStoreEvaluation,
    int Score,
    string? StrengthNotes,
    string? WeaknessNotes,
    string Recommendation
) : IRequest<Guid>;
