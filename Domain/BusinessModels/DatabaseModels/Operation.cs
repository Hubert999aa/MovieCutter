using Domain.BusinessEnums;

namespace Domain.BusinessModels.DatabaseModels
{
    public class Operation
    {
        public int IdOperation { get; set; }
        public string VideoName { get; set; } = string.Empty;
        public string VideoExtension { get; set; } = string.Empty;
        public OperationType OperationType { get; set; }
        public OperationStatus OperationStatus { get; set; } = OperationStatus.Queued;
        public VideoProcess VideoProcess { get; set; }
        public int ProgressPercentage { get; set; } = 0;
        public DateTime CreationnDate { get; set; } = DateTime.UtcNow;
    }
}
