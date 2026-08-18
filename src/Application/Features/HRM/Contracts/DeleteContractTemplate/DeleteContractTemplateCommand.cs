namespace Application;

public sealed record DeleteContractTemplateCommand(Guid TemplateId) : IRequest<Unit>;
