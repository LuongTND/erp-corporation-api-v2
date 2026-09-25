namespace Application;

public sealed record DeleteLabelCommand(Guid LabelId) : IRequest<Unit>;
