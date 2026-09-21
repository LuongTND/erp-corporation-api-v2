namespace Domain;

/// <summary>
/// Một vòng trong quy trình tuyển dụng.
/// IsFixed = true: vòng hệ thống (Ứng tuyển, Offer, Đã tuyển) — không cho xóa, không reorder ra ngoài vị trí cố định.
/// RoundTypeId = null khi IsFixed = true (các vòng fixed không thuộc loại vòng nào).
/// </summary>
public class RecruitmentPipelineStage : EntityBase<Guid>
{
    public Guid PipelineId { get; set; }
    public RecruitmentPipeline? Pipeline { get; set; }

    /// <summary>null khi IsFixed = true.</summary>
    public Guid? RoundTypeId { get; set; }
    public RoundType? RoundType { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    /// <summary>true = vòng hệ thống, không cho phép xóa (Ứng tuyển, Offer, Đã tuyển).</summary>
    public bool IsFixed { get; set; }
}
