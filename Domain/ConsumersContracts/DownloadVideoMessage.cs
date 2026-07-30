using Domain.BusinessModels.DatabaseModels;

namespace Domain.ConsumersContracts
{
    public class DownloadVideoMessage
    {
        public required string Url { get; set; }
        public required string OutputFolder { get; set; }
        public required Operation Operation { get; set; }
    }
}
