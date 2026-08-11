namespace Application;

public sealed record ExportUsersQuery(string? Search = null, UserStatus? Status = null, Guid? DepartmentId = null) : IRequest<byte[]>;
