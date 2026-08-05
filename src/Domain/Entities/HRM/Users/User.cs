namespace Domain;

public class User : AuditableEntityBase<Guid>
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }

    public Guid JobLevelId { get; set; }
    public JobLevel? JobLevel { get; set; }

    public Guid? ManagerId { get; set; }
    public User? Manager { get; set; }
    public ICollection<User> DirectReports { get; set; } = [];

    public DateOnly DateOfJoin { get; set; }
    public UserStatus Status { get; private set; }
    public bool IsActive { get; private set; }
    public ScopeType? ScopeOverride { get; set; }

    // Personal info (HRM-023)
    public Gender? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? IdentityCardNumber { get; set; }
    public DateOnly? IdentityCardIssuedDate { get; set; }
    public string? IdentityCardIssuedPlace { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PermanentAddress { get; set; }
    public string? CurrentAddress { get; set; }
    public string? TaxCode { get; set; }
    public string? SocialInsuranceCode { get; set; }
    public DateTimeOffset? ResignedAt { get; set; }
    public bool? HandoverCompleted { get; set; }

    public UserAccount? UserAccount { get; set; }
    public ICollection<UserDepartment> UserDepartments { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];

    public void ChangeStatus(UserStatus newStatus)
    {
        Status = newStatus;
        IsActive = newStatus is UserStatus.Active or UserStatus.Probation;
    }
}
