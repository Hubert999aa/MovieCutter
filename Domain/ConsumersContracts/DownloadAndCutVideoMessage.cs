using Domain.BusinessModels.DatabaseModels;

namespace Domain.ConsumersContracts
{
    public class DownloadAndCutVideoMessage
    {
        public required string Url { get; set; }
        public required string VideoOutputFolder { get; set; }
        public required string FramesOutputFolder { get; set; }
        public required bool CutVideoInOnePiece { get; set; }
        public required IEnumerable<VideoPiece> NewPices { get; set; }
        public required int IdOperation { get; set; }
    }
}
