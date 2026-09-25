namespace Application;

public sealed record AssignUserLabelCommand(Guid UserId, Guid LabelId) : IRequest<Unit>;
