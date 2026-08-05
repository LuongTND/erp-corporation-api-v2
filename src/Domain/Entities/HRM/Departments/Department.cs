namespace Domain;

public class Department : AuditableEntityBase<Guid>, ISoftDeletable
{
    public string DepartmentName { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public Guid? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }
    public ICollection<Department> ChildDepartments { get; set; } = [];

    public Guid? ManagerId { get; set; }
    public User? Manager { get; set; }

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<UserDepartment> UserDepartments { get; set; } = [];
}
