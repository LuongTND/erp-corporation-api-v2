namespace Application;

public sealed class GetUserDetailQueryHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage)
    : IRequestHandler<GetUserDetailQuery, UserDetailResponse>
{
    public async Task<UserDetailResponse> Handle(GetUserDetailQuery query, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>()
            .FindAsync(
                u => u.Id == query.UserId,
                ct,
                u => u.JobLevel!,
                u => u.Manager!,
                u => u.Profile!,
                u => u.Identity!,
                u => u.EmploymentInfo!,
                u => u.UserDepartments!)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", query.UserId));

        // Load departments + their managers (GetAllAsync has no includes overload)
        var departmentIds = user.UserDepartments.Select(ud => ud.DepartmentId).ToHashSet();
        var departments = await unitOfWork.Repository<Department>()
            .GetAllAsync(d => departmentIds.Contains(d.Id), ct);
        var departmentMap = departments.ToDictionary(d => d.Id);

        var deptManagerIds = departments.Where(d => d.ManagerId.HasValue).Select(d => d.ManagerId!.Value).ToHashSet();
        var deptManagers = await unitOfWork.Repository<User>()
            .GetAllAsync(u => deptManagerIds.Contains(u.Id), ct);
        var deptManagerMap = deptManagers.ToDictionary(u => u.Id);

        var account = await unitOfWork.Repository<UserAccount>()
            .FindAsync(a => a.UserId == query.UserId, ct);

        var rawValues = await unitOfWork.Repository<UserCustomFieldValue>()
            .GetAllAsync(v => v.UserId == query.UserId, ct);

        // Load definitions separately — GetAllAsync has no includes overload
        var definitionIds = rawValues.Select(v => v.DefinitionId).ToHashSet();
        var definitions = await unitOfWork.Repository<CustomFieldDefinition>()
            .GetAllAsync(d => definitionIds.Contains(d.Id) && d.IsActive, ct);
        var defMap = definitions.ToDictionary(d => d.Id);

        var customFieldValues = rawValues
            .Where(v => defMap.ContainsKey(v.DefinitionId))
            .Select(v => { v.Definition = defMap[v.DefinitionId]; return v; })
            .ToList();

        return MapToResponse(user, account, customFieldValues, departmentMap, deptManagerMap, blobStorage);
    }

    private const string Container = "avatars";

    private static UserDetailResponse MapToResponse(User user, UserAccount? account, IEnumerable<UserCustomFieldValue> customFields, Dictionary<Guid, Department> departmentMap, Dictionary<Guid, User> deptManagerMap, IBlobStorageService blobStorage)
        => new()
        {
            Id = user.Id,
            EmployeeCode = user.EmployeeCode,
            FullName = user.FullName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl is null ? null : blobStorage.GetUrl(Container, user.AvatarUrl),
            Status = user.Status.ToString(),
            IsActive = user.IsActive,
            IsLocked = account?.IsLocked ?? false,
            JobLevelId = user.JobLevelId,
            JobLevelName = user.JobLevel?.LevelName,
            ManagerId = user.ManagerId,
            ManagerName = user.Manager?.FullName,
            EmployeeTypeId = user.EmployeeTypeId,
            Profile = user.Profile is null ? null : new UserProfileDetailResponse
            {
                Gender = user.Profile.Gender?.ToString(),
                DateOfBirth = user.Profile.DateOfBirth,
                PhoneNumber = user.Profile.PhoneNumber,
                PermanentAddress = user.Profile.PermanentAddress,
                CurrentAddress = user.Profile.CurrentAddress,
            },
            Identity = user.Identity is null ? null : new UserIdentityDetailResponse
            {
                IdentityCardNumber = user.Identity.IdentityCardNumber,
                IdentityCardIssuedDate = user.Identity.IdentityCardIssuedDate,
                IdentityCardIssuedPlace = user.Identity.IdentityCardIssuedPlace,
                PassportNumber = user.Identity.PassportNumber,
                PassportExpiryDate = user.Identity.PassportExpiryDate,
            },
            Employment = user.EmploymentInfo is null ? null : new UserEmploymentDetailResponse
            {
                DateOfJoin = user.EmploymentInfo.DateOfJoin,
                ContractType = user.EmploymentInfo.ContractType?.ToString(),
                TaxCode = user.EmploymentInfo.TaxCode,
                SocialInsuranceCode = user.EmploymentInfo.SocialInsuranceCode,
                BankName = user.EmploymentInfo.BankName,
                BankAccountNumber = user.EmploymentInfo.BankAccountNumber,
                BankBranch = user.EmploymentInfo.BankBranch,
                ResignedAt = user.EmploymentInfo.ResignedAt,
                HandoverCompleted = user.EmploymentInfo.HandoverCompleted,
            },
            Departments = user.UserDepartments
                .Where(ud => ud.IsActive)
                .Select(ud => new UserDepartmentDetailResponse
                {
                    DepartmentId = ud.DepartmentId,
                    DepartmentName = departmentMap.TryGetValue(ud.DepartmentId, out var dept) ? dept.DepartmentName : string.Empty,
                    IsPrimary = ud.IsPrimary,
                    ManagerId = dept?.ManagerId,
                    ManagerName = dept?.ManagerId.HasValue == true && deptManagerMap.TryGetValue(dept.ManagerId.Value, out var mgr) ? mgr.FullName : null,
                }),
            CustomFields = customFields
                .OrderBy(v => v.Definition!.SortOrder)
                .Select(v => new CustomFieldValueResponse
                {
                    DefinitionId = v.DefinitionId,
                    Code = v.Definition!.Code,
                    Name = v.Definition.Name,
                    FieldType = v.Definition.FieldType.ToString(),
                    Group = v.Definition.Group,
                    SortOrder = v.Definition.SortOrder,
                    Value = v.Value,
                }),
        };
}
