namespace Domain;

/// <summary>
/// Profile cá nhân của người ứng tuyển.
/// Tách biệt với Application để một người có thể ứng tuyển nhiều vị trí khác nhau
/// mà không cần nhập lại thông tin cá nhân.
/// </summary>
public class Applicant : AuditableEntityBase<Guid>, ISoftDeletable
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public EducationLevel? EducationLevel { get; set; }
    public string? Notes { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<ApplicantDocument> Documents { get; set; } = [];
    public ICollection<Application> Applications { get; set; } = [];
}
