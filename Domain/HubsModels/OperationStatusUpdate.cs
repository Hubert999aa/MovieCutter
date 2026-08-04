namespace Domain.HubsModels
{
    public class OperationStatusUpdate
    {
        public required int IdOperation { get; set; }
        public required string OperationStatus { get; set; }
    }
}
