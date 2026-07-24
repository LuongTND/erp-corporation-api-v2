namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        var appAssembly = AssemblyReference.Assembly;

        TypeAdapterConfig.GlobalSettings.Scan(appAssembly);
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddValidatorsFromAssembly(appAssembly);

        services.AddServicesFromAssembly(appAssembly)
                .AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(appAssembly);
                    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformancePipelineBehavior<,>));
                    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
                });

        return services;
    }
}
