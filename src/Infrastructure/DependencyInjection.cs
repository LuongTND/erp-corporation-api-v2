using Application.Interfaces.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Implementations.Repositories;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection is not configured. Set CONNECTION_STRING in API/.env (mapped by EnvLoader).");
        }

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<Infrastructure.Persistence.Interceptors.AuditSaveChangesInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<Infrastructure.Persistence.Interceptors.AuditSaveChangesInterceptor>();
            options.UseSqlServer(connectionString)
                   .AddInterceptors(interceptor);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddManualInfrastructureRegistrations();
        services.AddScannedInfrastructureImplementations(typeof(DependencyInjection).Assembly);

        services.AddHostedService<OutboxProcessorHostedService>();

        return services;
    }
}
