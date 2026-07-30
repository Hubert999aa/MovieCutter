using Domain.BusinessEnums;

namespace Application.Interfaces
{
    public interface IOperationStatusManager
    {
        public void UpdateOperationStatus(OperationStatus status);
        public void UpdateOperationProgress(VideoProcess processType, int progeress);
    }
}
