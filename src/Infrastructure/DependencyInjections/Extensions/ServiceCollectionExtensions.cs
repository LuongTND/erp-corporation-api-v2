namespace Infrastructure;

public static class ServiceCollectionExtensions 
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var appConfig = new AppConfiguration(configuration);
        var jwtOptions = appConfig.GetJwtOptions();

        services.AddSingleton<IAppConfiguration>(appConfig);
        services.Configure<EmailOptions>(configuration.GetSection("EmailSettings"));

        services.AddSingleton(Options.Create(jwtOptions));

        services.AddHttpContextAccessor();
        services.AddHttpClient();

        services.AddInfrastructureDbContext(configuration)
                .AddJwtService(jwtOptions)
                .AddRedisCache(configuration)
                .AddAzureBlobStorage(configuration)
                // .AddQuartzService() // uncomment khi có scheduled jobs
                .AddServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        services.AddHostedService<OutboxProcessorHostedService>();


        return services;
    }
}
