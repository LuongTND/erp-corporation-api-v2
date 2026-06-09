using API.Base;
using Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireCrudPermissionAttribute : Attribute, IAsyncActionFilter
{
    private readonly CrudOperation _operation;

    public RequireCrudPermissionAttribute(CrudOperation operation)
    {
        _operation = operation;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is not ICrudPermissionProvider provider)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return;
        }

        var permissionCode = provider.Permissions.Get(_operation);
        var currentUserService = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();
        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        var userId = currentUserService.UserId;
        if (!userId.HasValue)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await authService.EnsurePermissionAsync(userId.Value, permissionCode);

        await next();
    }
}
