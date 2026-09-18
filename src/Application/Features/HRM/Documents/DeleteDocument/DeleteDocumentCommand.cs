namespace Application;

public sealed record DeleteDocumentCommand(
    Guid UserId,
    Guid DocumentId,
    bool IsHrDelete = false
) : IRequest<Unit>;
