namespace Application;

public sealed class CreateEmployeeCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
    : IRequestHandler<CreateEmployeeCommand, Guid>
{
    public async Task<Guid> Handle(CreateEmployeeCommand cmd, CancellationToken ct)
    {
        var emailTaken = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.Email == cmd.Email, ct);
        if (emailTaken)
            throw new ConflictException($"Email '{cmd.Email}' đã được sử dụng.");

        var jobLevelExists = await unitOfWork.Repository<JobLevel>()
            .AnyAsync(j => j.Id == cmd.JobLevelId && !j.IsDeleted, ct);
        if (!jobLevelExists)
            throw new NotFoundException(ExceptionMessages.NotFound("JobLevel", cmd.JobLevelId));

        if (cmd.ManagerId.HasValue)
        {
            var managerExists = await unitOfWork.Repository<User>()
                .AnyAsync(u => u.Id == cmd.ManagerId.Value && u.IsActive, ct);
            if (!managerExists)
                throw new NotFoundException(ExceptionMessages.NotFound("Manager", cmd.ManagerId.Value));
        }

        var employeeCode = cmd.EmployeeCode ?? await GenerateEmployeeCodeAsync(ct);

        var codeTaken = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.EmployeeCode == employeeCode, ct);
        if (codeTaken)
            throw new ConflictException($"Mã nhân viên '{employeeCode}' đã tồn tại.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            EmployeeCode = employeeCode,
            FullName = cmd.FullName,
            Email = cmd.Email,
            AvatarUrl = cmd.AvatarUrl,
            JobLevelId = cmd.JobLevelId,
            ManagerId = cmd.ManagerId,
            DateOfJoin = cmd.DateOfJoin,
            Gender = cmd.Gender,
            DateOfBirth = cmd.DateOfBirth,
            IdentityCardNumber = cmd.IdentityCardNumber,
            IdentityCardIssuedDate = cmd.IdentityCardIssuedDate,
            IdentityCardIssuedPlace = cmd.IdentityCardIssuedPlace,
            PhoneNumber = cmd.PhoneNumber,
            PermanentAddress = cmd.PermanentAddress,
            CurrentAddress = cmd.CurrentAddress,
            TaxCode = cmd.TaxCode,
            SocialInsuranceCode = cmd.SocialInsuranceCode
        };

        // ponytail: ChangeStatus sets IsActive, but User needs method call to init Status
        user.ChangeStatus(UserStatus.Active);

        var account = new UserAccount
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            LoginEmail = cmd.Email,
            // Default password = "Bahung@2025" — admin must reset after first login
            PasswordHash = passwordHasher.Hash("Bahung@2025"),
            IsLocked = false
        };

        await unitOfWork.Repository<User>().AddAsync(user);
        await unitOfWork.Repository<UserAccount>().AddAsync(account);
        await unitOfWork.EnsureSaveAsync(ct);

        return user.Id;
    }

    private async Task<string> GenerateEmployeeCodeAsync(CancellationToken ct)
    {
        // ponytail: in-memory max scan — switch to DB sequence if employee count > 10k
        var allCodes = await unitOfWork.Repository<User>()
            .GetAllAsync(u => u.EmployeeCode.StartsWith("NV"), ct);

        var maxNum = allCodes
            .Select(u => u.EmployeeCode[2..])
            .Where(s => int.TryParse(s, out _))
            .Select(s => int.Parse(s))
            .DefaultIfEmpty(0)
            .Max();

        return $"NV{(maxNum + 1):D4}";
    }
}
