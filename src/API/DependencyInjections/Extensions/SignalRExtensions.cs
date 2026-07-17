namespace API;

public static class SignalRExtensions
{
    public static IServiceCollection AddSignalRServices(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<IUserIdProvider, UserIdHubProvider>();
        services.AddScoped<INotificationRealtimeSender, SignalRNotificationSender>();
        return services;
    }
}
