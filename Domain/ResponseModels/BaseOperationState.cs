using Domain.BusinessEnums;

namespace Domain.ResponseModels
{
    public class BaseOperationState
    {
        public int IdOperation { get; set; }
        public string VideoName { get; set; }
        public string OperationType { get; set; }
        public string OperationStatus { get; set; }
        public int ProgressPercentage { get; set; }
    }
}
