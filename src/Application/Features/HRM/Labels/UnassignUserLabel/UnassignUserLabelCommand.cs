namespace Application;

public sealed record UnassignUserLabelCommand(Guid UserId, Guid LabelId) : IRequest<Unit>;
