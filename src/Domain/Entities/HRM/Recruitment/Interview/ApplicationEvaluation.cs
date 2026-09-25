namespace Domain;

/// <summary>
/// Đánh giá của interviewer sau một vòng phỏng vấn.
/// Gắn với InterviewSchedule để biết đánh giá thuộc vòng nào.
/// Recommendation = StrongHire/Hire/NoHire quyết định Application có pass vòng đó không.
/// </summary>
public class ApplicationEvaluation : AuditableEntityBase<Guid>
{
    public Guid ApplicationId { get; set; }
    public Application? Application { get; set; }

    public Guid? InterviewScheduleId { get; set; }
    public InterviewSchedule? InterviewSchedule { get; set; }

    public Guid EvaluatorId { get; set; }
    public User? Evaluator { get; set; }

    public int Score { get; set; }

    public string? StrengthNotes { get; set; }
    public string? WeaknessNotes { get; set; }

    public EvaluationRecommendation Recommendation { get; set; }
}
