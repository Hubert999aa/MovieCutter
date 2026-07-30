using Domain.BusinessEnums;

namespace Application.Interfaces
{
    public interface IOperationStatusManager
    {
        public void UpdateOperationStatus(int IdOperation, OperationStatus status);
        public void UpdateOperationProgress(int IdOperation, VideoProcess processType, int progeress);
    }
}
