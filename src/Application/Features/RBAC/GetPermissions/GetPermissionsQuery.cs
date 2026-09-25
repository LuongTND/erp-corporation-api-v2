namespace Application;

public sealed class GetPermissionsQuery : IRequest<QueryResult<PermissionResponse>>
{
    public string? SearchText { get; init; }
    public int Skip { get; init; } = 0;
    public int Top { get; init; } = 0; // 0 = return all
}
