namespace Domain;

public class Applicant : AuditableEntityBase<Guid>, ISoftDeletable
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<ApplicantDocument> Documents { get; set; } = [];
    public ICollection<Application> Applications { get; set; } = [];
}
