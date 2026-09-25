namespace Application;

public sealed record ToggleDocumentVisibilityCommand(
    Guid UserId,
    Guid DocumentId,
    bool IsVisibleToEmployee
) : IRequest<Unit>;
