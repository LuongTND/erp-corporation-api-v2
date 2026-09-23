namespace Domain;

public class ApplicantExperience : EntityBase<Guid>
{
    public Guid ApplicantId { get; set; }
    public Applicant? Applicant { get; set; }

    /// <summary>Nơi làm việc gần đây nhất (tên rút gọn hiển thị trên danh sách).</summary>
    public string? RecentWorkplace { get; set; }

    /// <summary>Tên công ty / nơi làm việc đầy đủ.</summary>
    public string CompanyName { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public string? Position { get; set; }
    public string? Description { get; set; }
}
