using Domain.BusinessEnums;

namespace JobsRunner.Interfaces
{
    public interface IOperationStatusManager
    {
        public Task UpdateOperationStatus(int IdOperation, OperationStatus status);
        public Task UpdateOperationProgress(int IdOperation, VideoProcess processType, int progress = 0);
        public Task StartAsync();
    }
}
