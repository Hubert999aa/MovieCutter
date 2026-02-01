namespace Domain.BusinessModels
{
    public class VideoMetadata
    {
        public required string Id { get; set; } = string.Empty;
        public required string Title { get; set; } = string.Empty;
        public required string Url { get; set; } = string.Empty;
    }
}
