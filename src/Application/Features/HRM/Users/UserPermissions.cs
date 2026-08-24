namespace Application;

public static class UserPermissions
{
    public const string Create = "hrm:users:create";
    public const string View = "hrm:users:view";
    public const string ViewHistory = "hrm:users:view-history";
    public const string Export = "hrm:users:export";
    public const string UpdateProfile = "hrm:users:update-profile";
    public const string UpdateCustomFields = "hrm:users:update-custom-fields";
    public const string UpdateStatus = "hrm:users:update-status";
    public const string Lock = "hrm:users:lock";
    public const string AssignEmployeeType = "hrm:users:assign-employee-type";
    public const string RemoveJobLevel = "hrm:users:remove-job-level";
    public const string AddDepartment = "hrm:users:add-department";
    public const string UpdateDepartment = "hrm:users:update-department";
    public const string RemoveDepartment = "hrm:users:remove-department";
    public const string TransferDepartment = "hrm:users:transfer-department";
    public const string AssignRole = "hrm:users:assign-role";
    public const string RevokeRole = "hrm:users:revoke-role";
    public const string SetScope = "hrm:users:set-scope";
}
