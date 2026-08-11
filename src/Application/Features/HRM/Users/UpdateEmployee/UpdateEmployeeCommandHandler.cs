namespace Application;

public sealed class UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEmployeeCommand cmd, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>()
            .FindTrackedAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        if (cmd.JobLevelId.HasValue && cmd.JobLevelId != user.JobLevelId)
        {
            var levelExists = await unitOfWork.Repository<JobLevel>()
                .AnyAsync(j => j.Id == cmd.JobLevelId.Value && !j.IsDeleted, ct);
            if (!levelExists)
                throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", cmd.JobLevelId.Value));
        }

        if (cmd.ManagerId.HasValue && cmd.ManagerId != user.ManagerId)
        {
            var managerExists = await unitOfWork.Repository<User>()
                .AnyAsync(u => u.Id == cmd.ManagerId.Value && u.IsActive, ct);
            if (!managerExists)
                throw new NotFoundException(ExceptionMessages.NotFound("Manager", cmd.ManagerId.Value));
        }

        var now = DateTimeOffset.UtcNow;
        var workLogs = new List<WorkHistory>();

        if (cmd.JobLevelId != user.JobLevelId)
        {
            var oldLevel = user.JobLevelId.HasValue ? await unitOfWork.Repository<JobLevel>().FindAsync(j => j.Id == user.JobLevelId.Value, ct) : null;
            var newLevel = cmd.JobLevelId.HasValue ? await unitOfWork.Repository<JobLevel>().FindAsync(j => j.Id == cmd.JobLevelId.Value, ct) : null;
            workLogs.Add(new WorkHistory { Id = Guid.NewGuid(), UserId = cmd.UserId, ChangeType = WorkHistoryChangeType.JobLevel, OldValue = oldLevel?.LevelName, NewValue = newLevel?.LevelName, ChangedBy = currentUser.UserId, ChangedAt = now });
        }

        if (cmd.ManagerId != user.ManagerId)
        {
            var oldManager = user.ManagerId.HasValue ? await unitOfWork.Repository<User>().FindAsync(u => u.Id == user.ManagerId.Value, ct) : null;
            var newManager = cmd.ManagerId.HasValue ? await unitOfWork.Repository<User>().FindAsync(u => u.Id == cmd.ManagerId.Value, ct) : null;
            workLogs.Add(new WorkHistory { Id = Guid.NewGuid(), UserId = cmd.UserId, ChangeType = WorkHistoryChangeType.Manager, OldValue = oldManager?.FullName, NewValue = newManager?.FullName, ChangedBy = currentUser.UserId, ChangedAt = now });
        }

        user.FullName = cmd.FullName;
        user.JobLevelId = cmd.JobLevelId;
        user.ManagerId = cmd.ManagerId;

        // Profile (upsert via tracked)
        var profile = await unitOfWork.Repository<EmployeeProfile>()
            .FindTrackedAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
        {
            await unitOfWork.Repository<EmployeeProfile>().AddAsync(new EmployeeProfile
            {
                Id = Guid.NewGuid(),
                UserId = cmd.UserId,
                Gender = cmd.Gender,
                DateOfBirth = cmd.DateOfBirth,
                PhoneNumber = cmd.PhoneNumber,
                PermanentAddress = cmd.PermanentAddress,
                CurrentAddress = cmd.CurrentAddress,
            });
        }
        else
        {
            profile.Gender = cmd.Gender;
            profile.DateOfBirth = cmd.DateOfBirth;
            profile.PhoneNumber = cmd.PhoneNumber;
            profile.PermanentAddress = cmd.PermanentAddress;
            profile.CurrentAddress = cmd.CurrentAddress;
        }

        // Identity (upsert via tracked)
        var identity = await unitOfWork.Repository<EmployeeIdentity>()
            .FindTrackedAsync(i => i.UserId == cmd.UserId, ct);

        if (identity is null)
        {
            await unitOfWork.Repository<EmployeeIdentity>().AddAsync(new EmployeeIdentity
            {
                Id = Guid.NewGuid(),
                UserId = cmd.UserId,
                IdentityCardNumber = cmd.IdentityCardNumber,
                IdentityCardIssuedDate = cmd.IdentityCardIssuedDate,
                IdentityCardIssuedPlace = cmd.IdentityCardIssuedPlace,
                PassportNumber = cmd.PassportNumber,
                PassportExpiryDate = cmd.PassportExpiryDate,
            });
        }
        else
        {
            identity.IdentityCardNumber = cmd.IdentityCardNumber;
            identity.IdentityCardIssuedDate = cmd.IdentityCardIssuedDate;
            identity.IdentityCardIssuedPlace = cmd.IdentityCardIssuedPlace;
            identity.PassportNumber = cmd.PassportNumber;
            identity.PassportExpiryDate = cmd.PassportExpiryDate;
        }

        // EmploymentInfo (upsert via tracked)
        var employment = await unitOfWork.Repository<EmploymentInfo>()
            .FindTrackedAsync(e => e.UserId == cmd.UserId, ct);

        if (employment is null)
        {
            await unitOfWork.Repository<EmploymentInfo>().AddAsync(new EmploymentInfo
            {
                Id = Guid.NewGuid(),
                UserId = cmd.UserId,
                DateOfJoin = cmd.DateOfJoin ?? DateOnly.FromDateTime(DateTime.UtcNow),
                ContractType = cmd.ContractType,
                TaxCode = cmd.TaxCode,
                SocialInsuranceCode = cmd.SocialInsuranceCode,
                BankName = cmd.BankName,
                BankAccountNumber = cmd.BankAccountNumber,
                BankBranch = cmd.BankBranch,
            });
        }
        else
        {
            if (employment.ContractType != cmd.ContractType)
                workLogs.Add(new WorkHistory { Id = Guid.NewGuid(), UserId = cmd.UserId, ChangeType = WorkHistoryChangeType.ContractType, OldValue = employment.ContractType?.ToString(), NewValue = cmd.ContractType?.ToString(), ChangedBy = currentUser.UserId, ChangedAt = now });

            if (cmd.DateOfJoin.HasValue)
                employment.DateOfJoin = cmd.DateOfJoin.Value;
            employment.ContractType = cmd.ContractType;
            employment.TaxCode = cmd.TaxCode;
            employment.SocialInsuranceCode = cmd.SocialInsuranceCode;
            employment.BankName = cmd.BankName;
            employment.BankAccountNumber = cmd.BankAccountNumber;
            employment.BankBranch = cmd.BankBranch;
        }

        foreach (var log in workLogs)
            await unitOfWork.Repository<WorkHistory>().AddAsync(log);

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
