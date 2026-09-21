namespace Contract;

public class RecruitmentPipelineResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
    public IEnumerable<RecruitmentPipelineStageResponse> Stages { get; init; } = [];
}

public class RecruitmentPipelineStageResponse
{
    public Guid Id { get; init; }
    public Guid? RoundTypeId { get; init; }
    public string? RoundTypeName { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public bool IsFixed { get; init; }
}
