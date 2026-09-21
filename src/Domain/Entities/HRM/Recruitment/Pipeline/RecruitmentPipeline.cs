namespace Domain;

/// <summary>
/// Template quy trình tuyển dụng — định nghĩa danh sách các vòng theo thứ tự.
/// Một công ty thường có 1 quy trình mặc định (IsDefault = true).
/// Khi tạo tin tuyển dụng, FE load quy trình mặc định để hiển thị.
/// </summary>
public class RecruitmentPipeline : EntityBase<Guid>
{
    public string Name { get; set; } = string.Empty;

    /// <summary>true = đây là quy trình mặc định, load sẵn khi tạo tin tuyển dụng.</summary>
    public bool IsDefault { get; set; }

    public ICollection<RecruitmentPipelineStage> Stages { get; set; } = [];
}
