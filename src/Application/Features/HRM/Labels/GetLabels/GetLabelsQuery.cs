namespace Application;

public sealed record GetLabelsQuery(string? Search = null, bool? IsActive = null) : IRequest<IEnumerable<LabelResponse>>;
