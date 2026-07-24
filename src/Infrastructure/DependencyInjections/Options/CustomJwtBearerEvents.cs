
namespace Infrastructure;

public class CustomJwtBearerEvents : JwtBearerEvents
{
    public override Task MessageReceived(MessageReceivedContext context)
    {
        if (context.Request.Cookies.TryGetValue("access_token", out var cookieToken))
        {
            context.Token = cookieToken;
        }
        else
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                context.Token = authHeader["Bearer ".Length..].Trim();
        }

        return Task.CompletedTask;
    }

    public override Task Challenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();
        throw new UnauthorizedException("Access token is missing or invalid.");
    }

    public override Task Forbidden(ForbiddenContext context)
        => throw new ForbiddenException(ExceptionMessages.Forbidden);
}
