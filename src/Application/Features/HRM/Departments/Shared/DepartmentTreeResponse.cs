namespace Application;

public sealed class DepartmentTreeResponse
{
    public Guid Id { get; init; }
    public string DepartmentName { get; init; } = string.Empty;
    public string DepartmentCode { get; init; } = string.Empty;
    public Guid? ParentDepartmentId { get; init; }
    public Guid? ManagerId { get; init; }
    public string? ManagerName { get; init; }
    public string? ManagerAvatarUrl { get; init; }
    public bool IsActive { get; init; }
    public int MemberCount { get; init; }
    public List<DepartmentTreeResponse> Children { get; init; } = [];
}
