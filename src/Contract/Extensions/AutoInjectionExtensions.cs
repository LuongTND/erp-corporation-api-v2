namespace Contract;

public static class AutoInjectionExtensions
{
    public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            if (assembly == null) continue;

            var types = assembly.GetTypes()
                .Where(type => type.IsClass
                            && !type.IsAbstract
                            && type.GetCustomAttribute<RegisterServiceAttribute>() != null);

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<RegisterServiceAttribute>();
                if (attribute == null) continue;

                if (!attribute.ServiceType.IsAssignableFrom(type))
                    throw new InvalidOperationException(
                        $"{type.Name} does not implement {attribute.ServiceType.Name}");

                switch (attribute.Lifetime)
                {
                    case ServiceLifetime.Transient:
                        services.AddTransient(attribute.ServiceType, type);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(attribute.ServiceType, type);
                        break;
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(attribute.ServiceType, type);
                        break;
                }
            }
        }
        return services;
    }
}
