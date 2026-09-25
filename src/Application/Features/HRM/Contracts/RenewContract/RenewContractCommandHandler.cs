namespace Application;

public sealed class RenewContractCommandHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage, IUserContext currentUser)
    : IRequestHandler<RenewContractCommand, Guid>
{
    private const string Container = "contract-files";

    public async Task<Guid> Handle(RenewContractCommand cmd, CancellationToken ct)
    {
        var old = await unitOfWork.Repository<EmploymentContract>()
            .FindTrackedAsync(c => c.Id == cmd.ContractId && c.UserId == cmd.UserId, ct);
        if (old is null)
            throw new NotFoundException(ExceptionMessages.NotFound("EmploymentContract", cmd.ContractId));

        if (old.Status != ContractStatus.Active)
            throw new BadRequestException("Chỉ có thể tái ký hợp đồng đang hiệu lực.");

        old.Status = ContractStatus.Renewed;
        old.ModifiedAt = DateTimeOffset.UtcNow;
        old.UpdatedBy = currentUser.UserId;

        var year = DateTimeOffset.UtcNow.Year;
        var prefix = $"HD-{year}-";
        var existing = await unitOfWork.Repository<EmploymentContract>()
            .GetAllAsync(c => c.ContractNumber.StartsWith(prefix), ct);
        var max = existing
            .Select(c => int.TryParse(c.ContractNumber[prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        var newNumber = $"{prefix}{(max + 1):D4}";

        var blobName = $"{cmd.UserId}/{newNumber}/{cmd.OriginalFileName}";
        var fileUrl = await blobStorage.UploadAsync(Container, blobName, cmd.FileStream, cmd.ContentType, ct: ct);

        var renewed = new EmploymentContract
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            ContractNumber = newNumber,
            Type = cmd.Type,
            Status = ContractStatus.Active,
            StartDate = cmd.StartDate,
            EndDate = cmd.EndDate,
            Salary = cmd.Salary,
            SalaryForSocialInsurance = cmd.SalaryForSocialInsurance,
            PositionTitle = cmd.PositionTitle,
            FileUrl = fileUrl,
            SignedDate = cmd.SignedDate,
            RenewedFromContractId = old.Id,
            CreatedBy = currentUser.UserId,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await unitOfWork.Repository<EmploymentContract>().AddAsync(renewed);
        await unitOfWork.EnsureSaveAsync(ct);
        return renewed.Id;
    }
}
