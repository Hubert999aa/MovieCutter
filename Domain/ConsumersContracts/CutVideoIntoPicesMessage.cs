namespace Domain.ConsumersContracts
{
    public class CutVideoIntoPicesMessage
    {
        public required IEnumerable<VideoPice> NewPices { get; set; }
        public required string SourceVideoPath { get; set; }
        public required string NewVideoPathWithoutExtension { get; set; }
        public required string NewVideoExtension { get; set; }
    }

    public class VideoPice
    {
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
    }
}
