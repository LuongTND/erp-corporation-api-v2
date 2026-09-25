namespace Application;

public sealed class CreateContractCommandHandler(
    IUnitOfWork unitOfWork,
    IBlobStorageService blobStorage,
    IUserContext currentUser)
    : IRequestHandler<CreateContractCommand, Guid>
{
    private const string Container = "contract-files";

    public async Task<Guid> Handle(CreateContractCommand cmd, CancellationToken ct)
    {
        var userExists = await unitOfWork.Repository<User>()
            .AnyAsync(u => u.Id == cmd.UserId && u.IsActive, ct);
        if (!userExists)
            throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        var hasActive = await unitOfWork.Repository<EmploymentContract>()
            .AnyAsync(c => c.UserId == cmd.UserId && c.Status == ContractStatus.Active, ct);
        if (hasActive)
            throw new BadRequestException("Nhân sự đã có hợp đồng đang hiệu lực. Vui lòng thanh lý hoặc tái ký trước.");

        var contractNumber = await GenerateContractNumberAsync(ct);

        var blobName = $"{cmd.UserId}/{contractNumber}/{cmd.OriginalFileName}";
        var fileUrl = await blobStorage.UploadAsync(Container, blobName, cmd.FileStream, cmd.ContentType, ct: ct);

        var contract = new EmploymentContract
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            ContractNumber = contractNumber,
            Type = cmd.Type,
            Status = ContractStatus.Active,
            StartDate = cmd.StartDate,
            EndDate = cmd.EndDate,
            Salary = cmd.Salary,
            SalaryForSocialInsurance = cmd.SalaryForSocialInsurance,
            PositionTitle = cmd.PositionTitle,
            FileUrl = fileUrl,
            SignedDate = cmd.SignedDate,
            TemplateId = cmd.TemplateId,
            CreatedBy = currentUser.UserId,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await unitOfWork.Repository<EmploymentContract>().AddAsync(contract);
        await unitOfWork.EnsureSaveAsync(ct);
        return contract.Id;
    }

    private async Task<string> GenerateContractNumberAsync(CancellationToken ct)
    {
        var year = DateTimeOffset.UtcNow.Year;
        var prefix = $"HD-{year}-";

        var existing = await unitOfWork.Repository<EmploymentContract>()
            .GetAllAsync(c => c.ContractNumber.StartsWith(prefix), ct);

        var max = existing
            .Select(c => int.TryParse(c.ContractNumber[prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"{prefix}{(max + 1):D4}";
    }
}
