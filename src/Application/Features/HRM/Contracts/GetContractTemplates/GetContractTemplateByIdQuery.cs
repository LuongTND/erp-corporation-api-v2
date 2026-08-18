namespace Application;

public sealed record GetContractTemplateByIdQuery(Guid Id) : IRequest<ContractTemplateDetailResult>;

public sealed record ContractTemplateDetailResult(string BlobName, string OriginalFileName);
