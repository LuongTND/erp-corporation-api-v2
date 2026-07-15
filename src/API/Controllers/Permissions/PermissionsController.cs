namespace API;

[Route("api/permissions")]
public class PermissionsController : CrudApiController<IPermissionService, PermissionDto, CreatePermissionRequest,
    UpdatePermissionRequest>
{
    public PermissionsController(IPermissionService permissionService)
        : base(permissionService, new CrudPermissions
        {
            Read = "system.permission.read",
            Create = "system.permission.create",
            Update = "system.permission.update",
            Delete = "system.permission.delete",
        })
    {
    }
}