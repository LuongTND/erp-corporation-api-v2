namespace Application;

public sealed record ExportUsersQuery(
    string? Search = null,
    UserStatus? Status = null,
    Guid? DepartmentId = null,
    Guid? LabelId = null,
    Guid? StoreId = null,
    Guid? RegionId = null,
    Guid CallerId = default) : IRequest<byte[]>;