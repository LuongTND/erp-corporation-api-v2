namespace Application;

public sealed record CreateLabelCommand(string Name, string Color) : IRequest<Guid>;
