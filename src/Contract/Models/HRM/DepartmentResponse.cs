namespace Contract;

public sealed class DepartmentResponse
{
    public Guid Id { get; init; }
    public string DepartmentName { get; init; } = string.Empty;
    public string DepartmentCode { get; init; } = string.Empty;
    public Guid? ParentDepartmentId { get; init; }
    public string? ParentDepartmentName { get; init; }
    public Guid? ManagerId { get; init; }
    public string? ManagerName { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}
