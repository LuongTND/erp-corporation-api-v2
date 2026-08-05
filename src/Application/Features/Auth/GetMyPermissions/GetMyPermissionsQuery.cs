namespace Application;

public sealed record GetMyPermissionsQuery : IRequest<IReadOnlyCollection<string>>;
