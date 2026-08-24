namespace Contract;

public sealed class RecruitmentApproverConfigResponse
{
    public Guid    Id             { get; init; }
    public Guid    ApproverId     { get; init; }
    public string  ApproverName   { get; init; } = string.Empty;
    public Guid?   DepartmentId   { get; init; }
    public string? DepartmentName { get; init; }
    public string? Note           { get; init; }
}
