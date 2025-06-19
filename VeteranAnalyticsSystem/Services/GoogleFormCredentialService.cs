using Azure.Storage.Blobs;
using VeteranAnalyticsSystem.Contracts;

namespace VeteranAnalyticsSystem.Services;

public class GoogleFormCredentialService(IConfiguration configuration) : IGoogleFormCredentialService
{
    private const string ContainerName = "credentials";
    private const string FileName = "credentials.json";

    public async Task<byte[]> DownloadCredentials()
    {
        var blobServicClient = new BlobServiceClient(configuration.GetConnectionString("AzureStorageConnection"));
        var containerClient = blobServicClient.GetBlobContainerClient(ContainerName);
        var blobClient = containerClient.GetBlobClient(FileName);

        using var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);

        memoryStream.Position = 0;

        return memoryStream.ToArray();
    }
}
