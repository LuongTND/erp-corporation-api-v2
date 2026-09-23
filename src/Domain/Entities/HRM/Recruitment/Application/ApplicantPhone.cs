namespace Domain;

public class ApplicantPhone : EntityBase<Guid>
{
    public Guid ApplicantId { get; set; }
    public Applicant? Applicant { get; set; }

    public string Phone { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}
