namespace VeteranAnalyticsSystem.Contracts;

public interface IGoogleFormCredentialService
{
    Task<byte[]> DownloadCredentials();
}
