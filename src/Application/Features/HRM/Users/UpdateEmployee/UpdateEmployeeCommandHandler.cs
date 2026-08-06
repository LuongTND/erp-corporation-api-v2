namespace Application;

public sealed class UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEmployeeCommand cmd, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>()
            .FindTrackedAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        if (cmd.JobLevelId != user.JobLevelId)
        {
            var levelExists = await unitOfWork.Repository<JobLevel>()
                .AnyAsync(j => j.Id == cmd.JobLevelId && !j.IsDeleted, ct);
            if (!levelExists)
                throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", cmd.JobLevelId));
        }

        if (cmd.ManagerId.HasValue && cmd.ManagerId != user.ManagerId)
        {
            var managerExists = await unitOfWork.Repository<User>()
                .AnyAsync(u => u.Id == cmd.ManagerId.Value && u.IsActive, ct);
            if (!managerExists)
                throw new NotFoundException(ExceptionMessages.NotFound("Manager", cmd.ManagerId.Value));
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
            if (cmd.DateOfJoin.HasValue)
                employment.DateOfJoin = cmd.DateOfJoin.Value;
            employment.ContractType = cmd.ContractType;
            employment.TaxCode = cmd.TaxCode;
            employment.SocialInsuranceCode = cmd.SocialInsuranceCode;
            employment.BankName = cmd.BankName;
            employment.BankAccountNumber = cmd.BankAccountNumber;
            employment.BankBranch = cmd.BankBranch;
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
