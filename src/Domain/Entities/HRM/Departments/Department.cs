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

    /// <summary>
    /// Role key của người phỏng vấn cho department này.
    /// null = kế thừa từ ParentDepartment (Khối cha).
    /// Ví dụ: Khối Sản Xuất set "production_manager", 8 dept con tự kế thừa.
    /// </summary>
    public string? InterviewerRoleKey { get; set; }

    /// <summary>
    /// Role key được thông báo sau khi ứng viên đạt (chốt Hired).
    /// null = kế thừa từ ParentDepartment.
    /// </summary>
    public string? NotifyAfterHiredRoleKey { get; set; }

    public ICollection<UserDepartment> UserDepartments { get; set; } = [];
}
