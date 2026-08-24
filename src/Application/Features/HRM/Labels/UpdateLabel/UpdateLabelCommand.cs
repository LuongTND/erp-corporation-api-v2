namespace Application;

public sealed record UpdateLabelCommand(Guid LabelId, string Name, string Color, bool IsActive) : IRequest<Unit>;
