using Azure.Storage.Blobs;

namespace Infrastructure;

internal static class AzureBlobStorageExtensions
{
    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"]
            ?? throw new InvalidOperationException("AzureBlobStorage:ConnectionString is required");

        services.AddSingleton(new BlobServiceClient(connectionString));
        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        return services;
    }
}
