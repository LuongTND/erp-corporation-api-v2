namespace Application;

public static class BonusPolicyPermissions
{
    [PermissionInfo("Xem chính sách thưởng", "Xem danh sách chính sách thưởng hiện hành")]
    public const string ViewList = "hrm:bonus-policies:view-list";

    [PermissionInfo("Tạo chính sách thưởng", "Tạo mới chính sách thưởng")]
    public const string Create = "hrm:bonus-policies:create";
}
