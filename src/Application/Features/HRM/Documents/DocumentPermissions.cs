namespace Application;

public static class DocumentPermissions
{
    [PermissionInfo("Xem tài liệu nhân sự", "Xem tài liệu đính kèm hồ sơ nhân sự")]
    public const string View = "hrm:documents:view";

    [PermissionInfo("Tải lên tài liệu", "Đính kèm tài liệu vào hồ sơ nhân sự")]
    public const string Upload = "hrm:documents:upload";

    [PermissionInfo("Xóa tài liệu", "Xóa tài liệu khỏi hồ sơ nhân sự")]
    public const string Delete = "hrm:documents:delete";

    [PermissionInfo("Điều chỉnh hiển thị tài liệu", "Bật/tắt hiển thị tài liệu với nhân viên")]
    public const string ToggleVisibility = "hrm:documents:toggle-visibility";
}
