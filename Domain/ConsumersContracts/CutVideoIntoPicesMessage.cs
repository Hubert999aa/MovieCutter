using Domain.BusinessModels.DatabaseModels;

namespace Domain.ConsumersContracts
{
    public class CutVideoIntoPicesMessage
    {
        public required IEnumerable<VideoPiece> NewPieces { get; set; }
        public required string DownloadFolderPath { get; set; }
        public required string SourceVideoPath { get; set; }
        public required Operation Operation { get; set; }
    }

    public class VideoPiece
    {
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
    }
}
