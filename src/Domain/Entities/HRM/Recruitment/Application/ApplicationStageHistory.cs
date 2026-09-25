namespace Domain;

/// <summary>
/// Audit log mỗi lần Application đổi stage.
/// Ghi lại ai đổi, khi nào, từ stage nào sang stage nào và lý do (nếu có).
/// Dùng để trace lịch sử xử lý hồ sơ và báo cáo thời gian xử lý từng bước.
/// </summary>
public class ApplicationStageHistory : EntityBase<Guid>
{
    public Guid ApplicationId { get; set; }
    public Application? Application { get; set; }

    public ApplicationStage FromStage { get; set; }
    public ApplicationStage ToStage { get; set; }

    public Guid ChangedByUserId { get; set; }
    public User? ChangedBy { get; set; }

    public string? Note { get; set; }

    public DateTimeOffset ChangedAt { get; set; }
}
