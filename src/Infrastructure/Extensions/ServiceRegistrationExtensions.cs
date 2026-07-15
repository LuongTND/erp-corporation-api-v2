using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

internal static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddManualInfrastructureRegistrations(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<INotificationRealtimeSender, NoOpNotificationRealtimeSender>();
        services.AddScoped<INotificationActorResolver, NotificationActorResolver>();
        return services;
    }

    public static IServiceCollection AddScannedInfrastructureImplementations(
        this IServiceCollection services,
        Assembly infrastructureAssembly)
    {
        var implementationTypes = infrastructureAssembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, Namespace: "Infrastructure" })
            .ToList();

        foreach (var implementation in implementationTypes)
        {
            if (implementation == typeof(UnitOfWork))
                continue;

            if (implementation.IsGenericTypeDefinition &&
                implementation.GetGenericTypeDefinition() == typeof(GenericRepository<>))
                continue;

            var serviceInterfaces = implementation.GetInterfaces()
                .Where(i => i.Namespace == "Application")
                .Where(i => i != typeof(IGenericRepository<>))
                .ToList();

            foreach (var serviceInterface in serviceInterfaces)
            {
                if (serviceInterface.IsGenericTypeDefinition)
                    continue;

                services.AddScoped(serviceInterface, implementation);
            }
        }

        return services;
    }
}