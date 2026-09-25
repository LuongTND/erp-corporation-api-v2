namespace Domain;

public class CandidateEvaluation : AuditableEntityBase<Guid>
{
    public Guid CandidateId { get; set; }
    public Candidate? Candidate { get; set; }

    public Guid EvaluatorId { get; set; }
    public User? Evaluator { get; set; }

    public bool IsStoreEvaluation { get; set; }

    public int Score { get; set; }

    public string? StrengthNotes { get; set; }
    public string? WeaknessNotes { get; set; }

    public EvaluationRecommendation Recommendation { get; set; }
}
