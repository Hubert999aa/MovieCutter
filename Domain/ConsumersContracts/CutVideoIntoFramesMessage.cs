using Domain.BusinessModels.DatabaseModels;

namespace Domain.ConsumersContracts
{
    public class CutVideoIntoFramesMessage
    {
        public required string SourceVideoPath { get; set; }
        public required string FramesFolderPath { get; set; }
        public required Operation Operation { get; set; }
    }
}
