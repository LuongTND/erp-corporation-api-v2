namespace Application;

public static class SalaryPermissions
{
    [PermissionInfo("Xem lương nhân sự", "Xem mức lương của nhân sự")]
    public const string View = "hrm:salary:view";

    [PermissionInfo("Thiết lập lương nhân sự", "Cập nhật mức lương cho nhân sự")]
    public const string Set = "hrm:salary:set";
}
