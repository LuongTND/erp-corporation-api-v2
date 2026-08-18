namespace Application;

public sealed class GetContractTemplatesQueryHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage)
    : IRequestHandler<GetContractTemplatesQuery, IEnumerable<ContractTemplateResponse>>
{
    private const string Container = "contract-templates";

    public async Task<IEnumerable<ContractTemplateResponse>> Handle(GetContractTemplatesQuery query, CancellationToken ct)
    {
        var templates = await unitOfWork.Repository<ContractTemplate>()
            .GetAllAsync(t => t.IsActive, ct);

        return templates
            .OrderByDescending(t => t.CreatedAt)
            .Select(t =>
            {
                var r = t.Adapt<ContractTemplateResponse>();
                r.FileUrl = blobStorage.GetUrl(Container, t.BlobName);
                return r;
            });
    }
}
