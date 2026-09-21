namespace Application;

public sealed record ReorderRoundTypesCommand(
    IEnumerable<ReorderRoundTypeItem> Items
) : IRequest<Unit>;

public sealed record ReorderRoundTypeItem(Guid Id, int DisplayOrder);
