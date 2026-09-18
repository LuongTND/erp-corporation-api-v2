namespace Contract;

public sealed class CandidateEvaluationResponse
{
    public Guid Id { get; init; }
    public Guid ApplicationId { get; init; }
    public Guid EvaluatorId { get; init; }
    public string EvaluatorName { get; init; } = string.Empty;
    public int Score { get; init; }
    public string? StrengthNotes { get; init; }
    public string? WeaknessNotes { get; init; }
    public string Recommendation { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}
