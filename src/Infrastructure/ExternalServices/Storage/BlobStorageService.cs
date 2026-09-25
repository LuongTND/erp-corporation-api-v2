using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Infrastructure;

public sealed class BlobStorageService(BlobServiceClient client) : IBlobStorageService
{
    private static readonly TimeSpan SasExpiry = TimeSpan.FromDays(365 * 5);

    public async Task<string> UploadAsync(string containerName, string blobName, Stream content, string contentType, bool publicContainer = false, CancellationToken ct = default)
    {
        var container = client.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

        var blob = container.GetBlobClient(blobName);
        await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

        return BuildSasUrl(blob);
    }

    public async Task<Stream> DownloadAsync(string containerName, string blobName, CancellationToken ct = default)
    {
        var response = await client.GetBlobContainerClient(containerName)
            .GetBlobClient(blobName)
            .DownloadStreamingAsync(cancellationToken: ct);
        return response.Value.Content;
    }

    public async Task DeleteAsync(string containerName, string blobName, CancellationToken ct = default)
        => await client.GetBlobContainerClient(containerName).GetBlobClient(blobName).DeleteIfExistsAsync(cancellationToken: ct);

    public async Task<bool> ExistsAsync(string containerName, string blobName, CancellationToken ct = default)
        => await client.GetBlobContainerClient(containerName).GetBlobClient(blobName).ExistsAsync(cancellationToken: ct);

    public string GetUrl(string containerName, string blobName)
        => BuildSasUrl(client.GetBlobContainerClient(containerName).GetBlobClient(blobName));

    private string BuildSasUrl(BlobClient blob)
    {
        var sas = new BlobSasBuilder(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.Add(SasExpiry))
        {
            BlobContainerName = blob.BlobContainerName,
            BlobName = blob.Name,
            Resource = "b",
        };
        return blob.GenerateSasUri(sas).ToString();
    }
}
