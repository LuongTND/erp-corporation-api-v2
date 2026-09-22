namespace Domain;

/// <summary>
/// Hồ sơ / tài liệu đính kèm của ứng viên (CV, bằng cấp, chứng chỉ...).
/// Gắn với Applicant (không phải Application) để dùng lại qua nhiều lần ứng tuyển.
/// </summary>
public class ApplicantDocument : AuditableEntityBase<Guid>
{
    public Guid ApplicantId { get; set; }
    public Applicant? Applicant { get; set; }

    public ApplicantDocumentType DocumentType { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}
