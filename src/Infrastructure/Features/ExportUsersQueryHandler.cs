using ClosedXML.Excel;

namespace Infrastructure;

public sealed class ExportUsersQueryHandler(ApplicationDbContext db)
    : IRequestHandler<ExportUsersQuery, byte[]>
{
    public async Task<byte[]> Handle(ExportUsersQuery query, CancellationToken ct)
    {
        var users = await db.Users
            .Where(u => (query.Status == null ? u.IsActive : u.Status == query.Status.Value)
                && (query.Search == null || u.FullName.Contains(query.Search) || u.EmployeeCode.Contains(query.Search))
                && (query.DepartmentId == null || u.UserDepartments.Any(ud => ud.DepartmentId == query.DepartmentId.Value && ud.IsActive)))
            .Include(u => u.JobLevel)
            .Include(u => u.EmploymentInfo)
            .Include(u => u.UserDepartments)
            .OrderBy(u => u.FullName)
            .ToListAsync(ct);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Danh sách nhân sự");

        // Header
        string[] headers = ["STT", "Mã NV", "Họ tên", "Email", "Chức danh", "Loại HĐ", "Trạng thái", "Ngày vào làm"];
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // Rows
        for (int i = 0; i < users.Count; i++)
        {
            var u = users[i];
            var row = i + 2;
            ws.Cell(row, 1).Value = i + 1;
            ws.Cell(row, 2).Value = u.EmployeeCode;
            ws.Cell(row, 3).Value = u.FullName;
            ws.Cell(row, 4).Value = u.Email;
            ws.Cell(row, 5).Value = u.JobLevel?.LevelName ?? "";
            ws.Cell(row, 6).Value = u.EmploymentInfo?.ContractType?.ToString() ?? "";
            ws.Cell(row, 7).Value = StatusLabel(u.Status);
            ws.Cell(row, 8).Value = u.EmploymentInfo?.DateOfJoin.ToString("dd/MM/yyyy") ?? "";

            if (i % 2 == 1)
                ws.Row(row).Cells().Style.Fill.BackgroundColor = XLColor.FromHtml("#F8F7FF");
        }

        ws.Columns().AdjustToContents();
        ws.SheetView.FreezeRows(1);

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private static string StatusLabel(UserStatus s) => s switch
    {
        UserStatus.Active         => "Đang làm việc",
        UserStatus.Probation      => "Thử việc",
        UserStatus.Apprentice     => "Học việc",
        UserStatus.Official       => "Chính thức",
        UserStatus.Suspended      => "Tạm nghỉ",
        UserStatus.MaternityLeave => "Thai sản",
        UserStatus.Resigned       => "Đã nghỉ",
        UserStatus.Terminated     => "Đã thôi việc",
        _                         => s.ToString(),
    };
}
