namespace Application;

public class DepartmentDto : IHasGuidId
{
    public Guid Id { get; set; }
    public string DepartmentName { get; set; } = null!;
    public string DepartmentCode { get; set; } = null!;
    public Guid? ParentDepartmentId { get; set; }
    public string? ParentDepartmentName { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}