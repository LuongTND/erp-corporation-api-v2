namespace Domain;

/// <summary>
/// Tin tuyển dụng — bản ghi kênh đăng từ một RecruitmentRequest đã được duyệt.
/// HR tự đăng bên ngoài (Facebook, Zalo...), hệ thống chỉ lưu thông tin để gom CV.
/// Một Request có thể có nhiều JobPosting trên nhiều kênh khác nhau.
/// </summary>
public class JobPosting : AuditableEntityBase<Guid>
{
    public Guid RecruitmentRequestId { get; set; }
    public RecruitmentRequest? RecruitmentRequest { get; set; }

    public RecruitmentChannel Channel { get; set; }

    /// <summary>Tiêu đề tin đăng hiển thị trên kênh ngoài (Facebook, Zalo...). VD: "Tuyển Nhân Viên Bán Hàng - CH Liên Chiểu".</summary>
    public string? Title { get; set; }

    /// <summary>Địa điểm làm việc cụ thể. VD: "Cửa hàng 559 Trần Cao Vân" hoặc "Văn phòng tầng 3 - 120 Phan Châu Trinh".</summary>
    public string? WorkLocation { get; set; }

    /// <summary>Link bài đăng bên ngoài (Facebook post, Zalo OA...), null khi Channel = Internal.</summary>
    public string? PostUrl { get; set; }

    /// <summary>Tiêu chí sàng lọc CV — HR dùng nội bộ khi xem hồ sơ. VD: "Cao 1m65+, biết pha chế, ưu tiên có kinh nghiệm F&B".</summary>
    public string? Requirements { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public User? AssignedTo { get; set; }

    public JobPostingStatus Status { get; set; } = JobPostingStatus.Draft;

    public DateTimeOffset? PostedAt { get; set; }
    public DateOnly? ExpiresAt { get; set; }

    public ICollection<Application> Applications { get; set; } = [];
}
