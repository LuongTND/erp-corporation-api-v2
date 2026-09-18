namespace Domain;

public class ApplicantDocument : AuditableEntityBase<Guid>
{
    public Guid ApplicantId { get; set; }
    public Applicant? Applicant { get; set; }

    public ApplicantDocumentType DocumentType { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}
