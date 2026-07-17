namespace API;

public static class CorsExtensions
{
    public static IServiceCollection AddDevCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DevFrontend", policy =>
                policy.WithOrigins("http://localhost:810", "https://localhost:810")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });
        return services;
    }
}
