namespace Domain;

public class ApplicantEmail : EntityBase<Guid>
{
    public Guid ApplicantId { get; set; }
    public Applicant? Applicant { get; set; }

    public string Email { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}
