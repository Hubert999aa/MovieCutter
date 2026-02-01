namespace Domain.ConsumersContracts
{
    public class DownloadVideoMessage
    {
        public required string Url { get; set; }
        public required string OutputFolder { get; set; }
    }
}
