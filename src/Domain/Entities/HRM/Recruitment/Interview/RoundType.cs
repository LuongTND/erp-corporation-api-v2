namespace Domain;

/// <summary>
/// Danh mục loại vòng tuyển dụng (Thi tuyển, Phỏng vấn, Phỏng vấn kỹ thuật...).
/// Dùng chung cho toàn hệ thống — HR tạo một lần, admin chọn khi cấu hình quy trình.
/// IsSystem = true nghĩa là loại mặc định của hệ thống, không cho phép xoá.
/// </summary>
public class RoundType : EntityBase<Guid>
{
    public string Name { get; set; } = string.Empty;

    /// <summary>true = loại mặc định hệ thống, không cho xoá.</summary>
    public bool IsSystem { get; set; } = false;

    public int DisplayOrder { get; set; }

    public ICollection<InterviewRuleConfigStep> Steps { get; set; } = [];
}
