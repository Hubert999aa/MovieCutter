namespace Domain.HubsModels
{
    public class OperationProgressUpdate
    {
        public required int IdOperation { get; set; }
        public required int ProgressPercent { get; set; }
    }
}
