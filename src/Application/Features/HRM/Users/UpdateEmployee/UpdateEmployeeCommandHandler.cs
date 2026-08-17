namespace Application;

public sealed class UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork, IUserContext currentUser)
    : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEmployeeCommand cmd, CancellationToken ct)
    {
        // Single tracked query with includes — replaces 4+ sequential FindTrackedAsync calls
        var user = await unitOfWork.Repository<User>()
            .Query(tracking: true)
            .Include(u => u.JobLevel)
            .Include(u => u.Manager)
            .Include(u => u.Profile)
            .Include(u => u.Identity)
            .Include(u => u.EmploymentInfo)
            .FirstOrDefaultAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        var now = DateTimeOffset.UtcNow;
        var workLogs = new List<WorkHistory>();

        // JobLevel: validate new + build audit log using already-loaded old name
        if (cmd.JobLevelId != user.JobLevelId)
        {
            string? newLevelName = null;
            if (cmd.JobLevelId.HasValue)
            {
                var newLevel = await unitOfWork.Repository<JobLevel>()
                    .FindAsync(j => j.Id == cmd.JobLevelId.Value && !j.IsDeleted, ct)
                    ?? throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", cmd.JobLevelId.Value));
                newLevelName = newLevel.LevelName;
            }
            workLogs.Add(new WorkHistory { Id = Guid.NewGuid(), UserId = cmd.UserId, ChangeType = WorkHistoryChangeType.JobLevel, OldValue = user.JobLevel?.LevelName, NewValue = newLevelName, ChangedBy = currentUser.UserId, ChangedAt = now });
        }

        // Manager: validate new + build audit log using already-loaded old name
        if (cmd.ManagerId != user.ManagerId)
        {
            string? newManagerName = null;
            if (cmd.ManagerId.HasValue)
            {
                var newManager = await unitOfWork.Repository<User>()
                    .FindAsync(u => u.Id == cmd.ManagerId.Value && u.IsActive, ct)
                    ?? throw new NotFoundException(ExceptionMessages.NotFound("Manager", cmd.ManagerId.Value));
                newManagerName = newManager.FullName;
            }
            workLogs.Add(new WorkHistory { Id = Guid.NewGuid(), UserId = cmd.UserId, ChangeType = WorkHistoryChangeType.Manager, OldValue = user.Manager?.FullName, NewValue = newManagerName, ChangedBy = currentUser.UserId, ChangedAt = now });
        }

        user.FullName = cmd.FullName;
        user.JobLevelId = cmd.JobLevelId;
        user.ManagerId = cmd.ManagerId;

        // Profile — skip entirely if no fields provided; only update non-null fields
        if (cmd.Gender != null || cmd.DateOfBirth != null || cmd.PhoneNumber != null ||
            cmd.PermanentAddress != null || cmd.CurrentAddress != null)
        {
            if (user.Profile is null)
            {
                await unitOfWork.Repository<EmployeeProfile>().AddAsync(new EmployeeProfile
                {
                    Id = Guid.NewGuid(), UserId = cmd.UserId,
                    Gender = cmd.Gender, DateOfBirth = cmd.DateOfBirth,
                    PhoneNumber = cmd.PhoneNumber, PermanentAddress = cmd.PermanentAddress,
                    CurrentAddress = cmd.CurrentAddress,
                });
            }
            else
            {
                if (cmd.Gender != null)           user.Profile.Gender           = cmd.Gender;
                if (cmd.DateOfBirth != null)       user.Profile.DateOfBirth       = cmd.DateOfBirth;
                if (cmd.PhoneNumber != null)       user.Profile.PhoneNumber       = cmd.PhoneNumber;
                if (cmd.PermanentAddress != null)  user.Profile.PermanentAddress  = cmd.PermanentAddress;
                if (cmd.CurrentAddress != null)    user.Profile.CurrentAddress    = cmd.CurrentAddress;
            }
        }

        // Identity — skip entirely if no fields provided; only update non-null fields
        if (cmd.IdentityCardNumber != null || cmd.IdentityCardIssuedDate != null ||
            cmd.IdentityCardIssuedPlace != null || cmd.PassportNumber != null || cmd.PassportExpiryDate != null)
        {
            if (user.Identity is null)
            {
                await unitOfWork.Repository<EmployeeIdentity>().AddAsync(new EmployeeIdentity
                {
                    Id = Guid.NewGuid(), UserId = cmd.UserId,
                    IdentityCardNumber = cmd.IdentityCardNumber, IdentityCardIssuedDate = cmd.IdentityCardIssuedDate,
                    IdentityCardIssuedPlace = cmd.IdentityCardIssuedPlace, PassportNumber = cmd.PassportNumber,
                    PassportExpiryDate = cmd.PassportExpiryDate,
                });
            }
            else
            {
                if (cmd.IdentityCardNumber != null)     user.Identity.IdentityCardNumber     = cmd.IdentityCardNumber;
                if (cmd.IdentityCardIssuedDate != null) user.Identity.IdentityCardIssuedDate = cmd.IdentityCardIssuedDate;
                if (cmd.IdentityCardIssuedPlace != null)user.Identity.IdentityCardIssuedPlace= cmd.IdentityCardIssuedPlace;
                if (cmd.PassportNumber != null)         user.Identity.PassportNumber         = cmd.PassportNumber;
                if (cmd.PassportExpiryDate != null)     user.Identity.PassportExpiryDate     = cmd.PassportExpiryDate;
            }
        }

        // Employment — skip entirely if no fields provided; only update non-null fields
        if (cmd.DateOfJoin != null || cmd.ContractType != null || cmd.TaxCode != null ||
            cmd.SocialInsuranceCode != null || cmd.BankName != null ||
            cmd.BankAccountNumber != null || cmd.BankBranch != null)
        {
            if (user.EmploymentInfo is null)
            {
                await unitOfWork.Repository<EmploymentInfo>().AddAsync(new EmploymentInfo
                {
                    Id = Guid.NewGuid(), UserId = cmd.UserId,
                    DateOfJoin = cmd.DateOfJoin ?? DateOnly.FromDateTime(DateTime.UtcNow),
                    ContractType = cmd.ContractType, TaxCode = cmd.TaxCode,
                    SocialInsuranceCode = cmd.SocialInsuranceCode, BankName = cmd.BankName,
                    BankAccountNumber = cmd.BankAccountNumber, BankBranch = cmd.BankBranch,
                });
            }
            else
            {
                if (cmd.ContractType != null && cmd.ContractType != user.EmploymentInfo.ContractType)
                    workLogs.Add(new WorkHistory { Id = Guid.NewGuid(), UserId = cmd.UserId, ChangeType = WorkHistoryChangeType.ContractType, OldValue = user.EmploymentInfo.ContractType?.ToString(), NewValue = cmd.ContractType.ToString(), ChangedBy = currentUser.UserId, ChangedAt = now });

                if (cmd.DateOfJoin != null)          user.EmploymentInfo.DateOfJoin          = cmd.DateOfJoin.Value;
                if (cmd.ContractType != null)        user.EmploymentInfo.ContractType        = cmd.ContractType;
                if (cmd.TaxCode != null)             user.EmploymentInfo.TaxCode             = cmd.TaxCode;
                if (cmd.SocialInsuranceCode != null) user.EmploymentInfo.SocialInsuranceCode = cmd.SocialInsuranceCode;
                if (cmd.BankName != null)            user.EmploymentInfo.BankName            = cmd.BankName;
                if (cmd.BankAccountNumber != null)   user.EmploymentInfo.BankAccountNumber   = cmd.BankAccountNumber;
                if (cmd.BankBranch != null)          user.EmploymentInfo.BankBranch          = cmd.BankBranch;
            }
        }

        foreach (var log in workLogs)
            await unitOfWork.Repository<WorkHistory>().AddAsync(log);

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
