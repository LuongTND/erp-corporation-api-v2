namespace Contract;

public static class AutoInjectionExtensions
{
    public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            if (assembly == null) continue;

            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<RegisterServiceAttribute>() != null);

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<RegisterServiceAttribute>()!;

                if (!attr.ServiceType.IsAssignableFrom(type))
                    throw new InvalidOperationException($"{type.Name} does not implement {attr.ServiceType.Name}");

                switch (attr.Lifetime)
                {
                    case ServiceLifetime.Transient: services.AddTransient(attr.ServiceType, type); break;
                    case ServiceLifetime.Scoped: services.AddScoped(attr.ServiceType, type); break;
                    case ServiceLifetime.Singleton: services.AddSingleton(attr.ServiceType, type); break;
                }
            }
        }

        return services;
    }
}