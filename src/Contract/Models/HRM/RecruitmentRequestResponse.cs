namespace Contract;

public class RecruitmentRequestResponse
{
    public Guid Id { get; init; }
    public string RequestCode { get; init; } = string.Empty;
    public string RequestContext { get; init; } = string.Empty;
    public Guid? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public Guid? StoreId { get; init; }
    public string? StoreName { get; init; }
    public string PositionTitle { get; init; } = string.Empty;
    public Guid RequestedByUserId { get; init; }
    public string RequestedByName { get; init; } = string.Empty;
    public int Headcount { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? JobDescription { get; init; }
    public DateOnly? RequiredByDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed class RecruitmentRequestDetailResponse : RecruitmentRequestResponse
{
    public string? RejectionNote { get; init; }
    public string? NeedMoreInfoNote { get; init; }
    public IEnumerable<JobPostingResponse> JobPostings { get; init; } = [];
}
