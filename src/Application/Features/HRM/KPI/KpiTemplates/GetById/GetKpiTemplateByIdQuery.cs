namespace Application;

public sealed record GetKpiTemplateByIdQuery(Guid Id) : IRequest<KpiTemplateResponse>;
