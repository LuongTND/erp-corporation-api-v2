namespace API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSignalRServices();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

        services.AddEndpointsApiExplorer();

        services
            .AddJwtAuthentication(configuration)
            .AddDevCorsPolicy()
            .AddSwaggerWithJwtSecurity();

        return services;
    }
}
