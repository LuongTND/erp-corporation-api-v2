using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure;

[RegisterService(typeof(IAuthorizationHandler), ServiceLifetime.Transient)]
public sealed class PermissionAuthorizationHandler(IPermissionService permissionService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdValue = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(userIdValue, out var userId)) return;

        var permissions = await permissionService.GetPermissionsAsync(userId);

        if (permissions.Contains(requirement.Permission))
            context.Succeed(requirement);
    }
}
