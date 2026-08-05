namespace JobsRunner.Interfaces
{
    public interface IVideoDownloader
    {
        public Task<bool> DownloadVideoAsync(string videoUrl, string outputFolder, int operationId, CancellationToken cancellationToken);
        public Task<bool> DownloadVideoNameAndExtensionAsync(string videoUrl, int operationId, CancellationToken cancellationToken);
    }
}
