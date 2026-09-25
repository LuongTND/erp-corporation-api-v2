namespace Application;

public sealed class DeleteContractTemplateCommandHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage)
    : IRequestHandler<DeleteContractTemplateCommand, Unit>
{
    public async Task<Unit> Handle(DeleteContractTemplateCommand cmd, CancellationToken ct)
    {
        var template = await unitOfWork.Repository<ContractTemplate>()
            .FindTrackedAsync(t => t.Id == cmd.TemplateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound(nameof(ContractTemplate), cmd.TemplateId));

        await blobStorage.DeleteAsync("contract-templates", template.BlobName, ct);
        await unitOfWork.Repository<ContractTemplate>().RemoveAsync(template);
        await unitOfWork.EnsureSaveAsync(ct);

        return Unit.Value;
    }
}
