namespace Contract;

public sealed class UserDetailResponse
{
    public Guid Id { get; init; }
    public string EmployeeCode { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsLocked { get; init; }

    public Guid? JobLevelId { get; init; }
    public string? JobLevelName { get; init; }
    public Guid? ManagerId { get; init; }
    public string? ManagerName { get; init; }

    public UserProfileDetailResponse? Profile { get; init; }
    public UserIdentityDetailResponse? Identity { get; init; }
    public UserEmploymentDetailResponse? Employment { get; init; }
    public IEnumerable<UserDepartmentDetailResponse> Departments { get; init; } = [];
    public IEnumerable<CustomFieldValueResponse> CustomFields { get; init; } = [];
}

public sealed class UserDepartmentDetailResponse
{
    public Guid DepartmentId { get; init; }
    public string DepartmentName { get; init; } = string.Empty;
    public bool IsPrimary { get; init; }
    public Guid? ManagerId { get; init; }
    public string? ManagerName { get; init; }
}

public sealed class UserProfileDetailResponse
{
    public string? Gender { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public string? PhoneNumber { get; init; }
    public string? PermanentAddress { get; init; }
    public string? CurrentAddress { get; init; }
}

public sealed class UserIdentityDetailResponse
{
    public string? IdentityCardNumber { get; init; }
    public DateOnly? IdentityCardIssuedDate { get; init; }
    public string? IdentityCardIssuedPlace { get; init; }
    public string? PassportNumber { get; init; }
    public DateOnly? PassportExpiryDate { get; init; }
}

public sealed class UserEmploymentDetailResponse
{
    public DateOnly DateOfJoin { get; init; }
    public string? ContractType { get; init; }
    public string? TaxCode { get; init; }
    public string? SocialInsuranceCode { get; init; }
    public string? BankName { get; init; }
    public string? BankAccountNumber { get; init; }
    public string? BankBranch { get; init; }
    public DateTimeOffset? ResignedAt { get; init; }
    public bool? HandoverCompleted { get; init; }
}

public sealed class CustomFieldValueResponse
{
    public Guid DefinitionId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string FieldType { get; init; } = string.Empty;
    public string? Group { get; init; }
    public int SortOrder { get; init; }
    public string Value { get; init; } = string.Empty;
}
