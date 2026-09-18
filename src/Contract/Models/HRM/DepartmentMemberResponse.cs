namespace Contract;

public sealed class DepartmentMemberResponse
{
    public Guid UserDepartmentId { get; init; }
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string EmployeeCode { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public Guid? JobTitleId { get; init; }
    public string? JobName { get; init; }
    public bool IsPrimary { get; init; }
    public DateOnly StartDate { get; init; }
}
