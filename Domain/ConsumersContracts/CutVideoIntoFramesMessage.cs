namespace Domain.ConsumersContracts
{
    public class CutVideoIntoFramesMessage
    {
        public required string SourceVideoPath { get; set; }
        public required string OutputFolder { get; set; }
        public required string VideoName { get; set; }
    }
}
