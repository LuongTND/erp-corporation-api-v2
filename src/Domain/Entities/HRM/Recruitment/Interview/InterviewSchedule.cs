namespace Domain;

/// <summary>
/// Lịch phỏng vấn của một Application — bước "Sắp xếp lịch" trong pipeline.
/// HR tạo thủ công: chọn interviewer, địa điểm, thời gian.
/// Interviewer thực hiện phỏng vấn rồi điền kết quả → Application chuyển sang Interviewed.
/// </summary>
public class InterviewSchedule : AuditableEntityBase<Guid>
{
    public Guid ApplicationId { get; set; }
    public Application? Application { get; set; }

    public DateTimeOffset ScheduledAt { get; set; }

    /// <summary>Địa điểm cụ thể: "Tại CH Liên Chiểu", "Văn phòng tầng 3"...</summary>
    public string? LocationNote { get; set; }

    public string? Notes { get; set; }

    public InterviewScheduleStatus Status { get; set; } = InterviewScheduleStatus.Scheduled;

    public Guid? InterviewerId { get; set; }
    public User? Interviewer { get; set; }

    public string? InterviewResult { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public ICollection<ApplicationEvaluation> Evaluations { get; set; } = [];
}
