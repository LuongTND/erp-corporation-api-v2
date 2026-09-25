namespace Application;

public sealed class UploadContractTemplateCommandHandler(
    IUnitOfWork unitOfWork,
    IBlobStorageService blobStorage,
    IUserContext currentUser)
    : IRequestHandler<UploadContractTemplateCommand, ContractTemplateResponse>
{
    private const string Container = "contract-templates";

    public async Task<ContractTemplateResponse> Handle(UploadContractTemplateCommand cmd, CancellationToken ct)
    {
        var blobName = $"{Guid.NewGuid()}/{cmd.OriginalFileName}";
        await blobStorage.UploadAsync(Container, blobName, cmd.FileStream, cmd.ContentType, ct: ct);

        var template = new ContractTemplate
        {
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            Description = cmd.Description,
            BlobName = blobName,
            OriginalFileName = cmd.OriginalFileName,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = currentUser.UserId,
        };

        await unitOfWork.Repository<ContractTemplate>().AddAsync(template);
        await unitOfWork.EnsureSaveAsync(ct);

        var response = template.Adapt<ContractTemplateResponse>();
        response.FileUrl = blobStorage.GetUrl(Container, blobName);
        return response;
    }
}
