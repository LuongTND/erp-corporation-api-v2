namespace Application;

public sealed record GetContractTemplatesQuery : IRequest<IEnumerable<ContractTemplateResponse>>;
