namespace Domain;

public enum ApplicationStage
{
    /// <summary>Tiếp nhận &amp; tạo hồ sơ — HR upload CV, nhập thông tin ứng viên.</summary>
    New = 1,

    /// <summary>Sắp xếp lịch — đã hẹn lịch phỏng vấn, chờ thực hiện.</summary>
    Scheduled = 2,

    /// <summary>Đánh giá &amp; phỏng vấn — đã phỏng vấn, chờ chốt kết quả.</summary>
    Interviewed = 3,

    /// <summary>Chốt kết quả — đạt, đã hẹn lịch học việc / onboard.</summary>
    Hired = 4,

    /// <summary>Không đạt — từ bất kỳ bước nào.</summary>
    Rejected = 5,
}
