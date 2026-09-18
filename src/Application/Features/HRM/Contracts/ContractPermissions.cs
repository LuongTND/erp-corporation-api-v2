namespace Application;

public static class ContractPermissions
{
    [PermissionInfo("Xem hợp đồng", "Xem thông tin hợp đồng lao động của nhân sự")]
    public const string View = "hrm:contract:view";

    [PermissionInfo("Tạo hợp đồng", "Tạo mới hợp đồng lao động")]
    public const string Create = "hrm:contract:create";

    [PermissionInfo("Gia hạn hợp đồng", "Gia hạn hợp đồng lao động sắp hết hạn")]
    public const string Renew = "hrm:contract:renew";

    [PermissionInfo("Chấm dứt hợp đồng", "Kết thúc hợp đồng lao động")]
    public const string Terminate = "hrm:contract:terminate";
}

public static class ContractTemplatePermissions
{
    [PermissionInfo("Xem mẫu hợp đồng", "Xem danh sách và nội dung mẫu hợp đồng")]
    public const string View = "hrm:contract-templates:view";

    [PermissionInfo("Tải lên mẫu hợp đồng", "Đính kèm file mẫu hợp đồng")]
    public const string Upload = "hrm:contract-templates:upload";

    [PermissionInfo("Tải xuống mẫu hợp đồng", "Tải file mẫu hợp đồng về máy")]
    public const string Download = "hrm:contract-templates:download";

    [PermissionInfo("Xóa mẫu hợp đồng", "Xóa mẫu hợp đồng khỏi hệ thống")]
    public const string Delete = "hrm:contract-templates:delete";
}
