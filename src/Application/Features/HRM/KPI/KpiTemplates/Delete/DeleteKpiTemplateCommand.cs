namespace Application;

public sealed record DeleteKpiTemplateCommand(Guid Id) : IRequest<Unit>;
