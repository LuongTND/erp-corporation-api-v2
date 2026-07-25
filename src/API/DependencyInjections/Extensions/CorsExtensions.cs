namespace API;

public static class CorsExtensions
{
    public static IServiceCollection AddDevCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DevFrontend", policy =>
                policy.WithOrigins(
                        "http://localhost:810", "https://localhost:810",
                        "http://localhost:3000", "https://localhost:3000",
                        "http://localhost:5173", "https://localhost:5173",
                        "http://localhost:7001", "https://localhost:7001")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });
        return services;
    }
}
