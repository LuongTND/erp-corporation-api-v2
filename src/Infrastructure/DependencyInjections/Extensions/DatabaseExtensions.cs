namespace Infrastructure;

internal static class DatabaseExtensions
{
    public static IServiceCollection AddInfrastructureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<TimestampInterceptor>();
        services.AddSingleton<AuditInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
            options.AddInterceptors(
                sp.GetRequiredService<TimestampInterceptor>(),
                sp.GetRequiredService<AuditInterceptor>());
            options.ConfigureWarnings(w =>
                w.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
        });
        return services;
    }
}
