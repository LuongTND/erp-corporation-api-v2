namespace Application;

public sealed record DeleteDocumentCommand(Guid UserId, Guid DocumentId) : IRequest<Unit>;
