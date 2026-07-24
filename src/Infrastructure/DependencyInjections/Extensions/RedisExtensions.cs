using StackExchange.Redis;

namespace Infrastructure;

internal static class RedisExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Upstash:ConnectionString"]
            ?? throw new InvalidOperationException("Upstash:ConnectionString is required");

        var multiplexer = ConnectionMultiplexer.Connect(connectionString);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        return services;
    }
}
