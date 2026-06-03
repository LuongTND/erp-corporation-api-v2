using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

internal static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Đăng ký thủ công các dịch vụ cốt lõi (ưu tiên trước scan).
    /// </summary>
    public static IServiceCollection AddManualInfrastructureRegistrations(this IServiceCollection services)
    {
        services.AddScoped(typeof(Application.Interfaces.Repositories.IGenericRepository<>),
            typeof(Implementations.Repositories.GenericRepository<>));

        return services;
    }

    /// <summary>
    /// Scan có kiểm soát: chỉ class trong <c>Infrastructure.Implementations</c>
    /// implement interface thuộc <c>Application.Interfaces</c>.
    /// </summary>
    public static IServiceCollection AddScannedInfrastructureImplementations(
        this IServiceCollection services,
        Assembly infrastructureAssembly)
    {
        const string implementationsNamespace = "Infrastructure.Implementations";
        const string interfacesNamespace = "Application.Interfaces";

        var implementationTypes = infrastructureAssembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.Namespace?.StartsWith(implementationsNamespace, StringComparison.Ordinal) == true)
            .ToList();

        foreach (var implementation in implementationTypes)
        {
            if (implementation == typeof(Implementations.Repositories.UnitOfWork))
                continue;

            if (implementation.IsGenericTypeDefinition &&
                implementation.GetGenericTypeDefinition() == typeof(Implementations.Repositories.GenericRepository<>))
                continue;

            var serviceInterfaces = implementation.GetInterfaces()
                .Where(i => i.Namespace?.StartsWith(interfacesNamespace, StringComparison.Ordinal) == true)
                .Where(i => i != typeof(Application.Interfaces.Repositories.IGenericRepository<>))
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
