namespace Contract;

public sealed record UserStatusHistoryResponse(
    DateTimeOffset ChangedAt,
    string OldStatus,
    string NewStatus,
    string? Note,
    Guid? ChangedBy
);
