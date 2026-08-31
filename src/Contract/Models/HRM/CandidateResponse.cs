namespace Contract;

public class CandidateResponse
{
    public Guid Id { get; init; }
    public Guid? RecruitmentRequestId { get; init; }
    public string? RequestCode { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? CvUrl { get; init; }
    public string SourceChannel { get; init; } = string.Empty;
    public string Stage { get; init; } = string.Empty;
    public string? RejectionReason { get; init; }
    public int? EvaluationScore { get; init; }
    public string? EvaluationRecommendation { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed class CandidateDetailResponse : CandidateResponse
{
    public string? Notes { get; init; }
    public Guid? ConvertedEmployeeId { get; init; }
    public IEnumerable<CandidateEvaluationResponse> Evaluations { get; init; } = [];
}
