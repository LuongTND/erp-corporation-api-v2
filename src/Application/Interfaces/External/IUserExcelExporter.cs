namespace Application;

public interface IUserExcelExporter
{
    Task<byte[]> ExportAsync(ExportUsersQuery query, CancellationToken ct = default);
}
