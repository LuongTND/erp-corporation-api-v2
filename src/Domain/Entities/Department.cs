using Domain.Base;

namespace Domain.Entities;

public class Department : BaseEntity, IAuditable, ICreationTracked, IModificationTracked
{
    public string DepartmentName { get; private set; } = null!;
    public string DepartmentCode { get; private set; } = null!;
    public Guid? ParentDepartmentId { get; private set; }
    public virtual Department? ParentDepartment { get; private set; }
    public virtual ICollection<Department> ChildDepartments { get; private set; } = [];

    public Guid? ManagerId { get; private set; }
    public virtual User? Manager { get; private set; }

    public string? Description { get; private set; }

    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Navigation properties
    public virtual ICollection<User> Users { get; private set; } = [];
    public virtual ICollection<UserDepartment> UserDepartments { get; private set; } = [];

    private Department() : base()
    {
    }

    public static Department Create(
        string departmentName,
        string departmentCode,
        Guid? parentDepartmentId = null,
        Guid? managerId = null,
        string? description = null)
    {
        return new Department
        {
            DepartmentName = departmentName,
            DepartmentCode = departmentCode,
            ParentDepartmentId = parentDepartmentId,
            ManagerId = managerId,
            Description = description,
            IsActive = true
        };
    }

    public void Update(
        string departmentName,
        string departmentCode,
        Guid? parentDepartmentId = null,
        Guid? managerId = null,
        string? description = null)
    {
        DepartmentName = departmentName;
        DepartmentCode = departmentCode;
        ParentDepartmentId = parentDepartmentId;
        ManagerId = managerId;
        Description = description;
    }

    public void SetManager(Guid? managerId)
    {
        ManagerId = managerId;
    }
}
