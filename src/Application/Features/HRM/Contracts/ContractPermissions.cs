namespace Application;

public static class ContractPermissions
{
    public const string View = "hrm:contract:view";
    public const string Create = "hrm:contract:create";
    public const string Renew = "hrm:contract:renew";
    public const string Terminate = "hrm:contract:terminate";
}

public static class ContractTemplatePermissions
{
    public const string View = "hrm:contract-templates:view";
    public const string Upload = "hrm:contract-templates:upload";
    public const string Download = "hrm:contract-templates:download";
    public const string Delete = "hrm:contract-templates:delete";
}
