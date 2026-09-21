namespace Application;

public sealed record GetRoundTypesQuery : IRequest<IEnumerable<RoundTypeResponse>>;
