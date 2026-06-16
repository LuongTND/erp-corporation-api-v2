using Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizePermissionAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _permissionCode;

    public AuthorizePermissionAttribute(string permissionCode)
    {
        _permissionCode = permissionCode;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var currentUserService = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();
        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        var userId = currentUserService.UserId;
        if (!userId.HasValue)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await authService.EnsurePermissionAsync(userId.Value, _permissionCode);

        await next();
    }
}
