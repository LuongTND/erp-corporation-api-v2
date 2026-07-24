namespace API;

public static class SignalRExtensions
{
    public static IServiceCollection AddSignalRServices(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<IUserIdProvider, UserIdProvider>();
        services.AddServicesFromAssembly(typeof(SignalRExtensions).Assembly);
        return services;
    }

    public static WebApplication MapHubs(this WebApplication app)
    {
        app.MapHub<ChatHub>(ChatHub.HubPath);
        app.MapHub<NotificationHub>(NotificationHub.HubPath);
        return app;
    }
}
