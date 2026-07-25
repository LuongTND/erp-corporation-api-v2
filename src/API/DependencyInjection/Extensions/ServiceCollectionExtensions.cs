namespace API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = ctx =>
            {
                var errors = ctx.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());
                var response = ApiResponse<object>.Fail("Validation failed", 400, errors);
                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
            };
        });

        services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("SqlServerConnection")!)
                .AddRedis(configuration["Upstash:ConnectionString"]!);

        services.AddSignalRServices();

        services.AddCORSPolicy(configuration)
                .AddRateLimiting(configuration)
                .AddSwaggerWithJwtSecurity()
                .AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }
}
