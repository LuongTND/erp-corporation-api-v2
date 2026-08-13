namespace Application;

public sealed class ExportUsersQueryHandler(IUserExcelExporter exporter)
    : IRequestHandler<ExportUsersQuery, byte[]>
{
    public Task<byte[]> Handle(ExportUsersQuery query, CancellationToken ct)
        => exporter.ExportAsync(query, ct);
}
