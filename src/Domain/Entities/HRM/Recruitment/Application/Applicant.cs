namespace Domain;

/// <summary>
/// Profile cá nhân của người ứng tuyển.
/// Tách biệt với Application để một người có thể ứng tuyển nhiều vị trí khác nhau
/// mà không cần nhập lại thông tin cá nhân.
/// </summary>
public class Applicant : AuditableEntityBase<Guid>, ISoftDeletable
{
    public string FullName { get; set; } = string.Empty;
    public Gender? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Province { get; set; }
    public string? Ward { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<ApplicantPhone> Phones { get; set; } = [];
    public ICollection<ApplicantEmail> Emails { get; set; } = [];
    public ICollection<ApplicantEducation> Educations { get; set; } = [];
    public ICollection<ApplicantExperience> Experiences { get; set; } = [];
    public ICollection<ApplicantDocument> Documents { get; set; } = [];
    public ICollection<Application> Applications { get; set; } = [];
}
