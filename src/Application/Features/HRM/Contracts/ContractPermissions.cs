namespace Application;

public static class ContractPermissions
{
    public const string View = "contract:view";
    public const string Create = "contract:create";
    public const string Renew = "contract:renew";
    public const string Terminate = "contract:terminate";
}

public static class ContractTemplatePermissions
{
    public const string View = "contract-templates:view";
    public const string Upload = "contract-templates:upload";
    public const string Download = "contract-templates:download";
    public const string Delete = "contract-templates:delete";
}
