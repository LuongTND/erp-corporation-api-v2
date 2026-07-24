namespace API;

public static class CorsServiceCollectionExtensions
{
    public static IServiceCollection AddCORSPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()
            ?? throw new InvalidOperationException("Can't find AllowedOrigins config");

        var validOrigins = allowedOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .ToArray();

        if (validOrigins.Length == 0)
            throw new InvalidOperationException("AllowedOrigins must contain at least one valid origin");

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(validOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
        return services;
    }
}
