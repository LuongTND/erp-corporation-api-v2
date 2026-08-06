namespace Application;

public sealed class GetUserDetailQueryHandler(IUnitOfWork unitOfWork)
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
                u => u.EmploymentInfo!)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", query.UserId));

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

        return MapToResponse(user, customFieldValues);
    }

    private static UserDetailResponse MapToResponse(User user, IEnumerable<UserCustomFieldValue> customFields)
        => new()
        {
            Id = user.Id,
            EmployeeCode = user.EmployeeCode,
            FullName = user.FullName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            Status = user.Status.ToString(),
            IsActive = user.IsActive,
            JobLevelId = user.JobLevelId,
            JobLevelName = user.JobLevel?.LevelName,
            ManagerId = user.ManagerId,
            ManagerName = user.Manager?.FullName,
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
