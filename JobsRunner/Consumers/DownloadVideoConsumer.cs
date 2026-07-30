using Application.Interfaces;
using Domain.BusinessEnums;
using Domain.ConsumersContracts;
using JobsRunner.Interfaces;
using MassTransit;

namespace JobsRunner.Consumers
{
    public class DownloadVideoConsumer(IOperationStatusManager _operationStatusManager, IVideoDownloader _videoDownloader) : IConsumer<DownloadVideoMessage>
    {
        public async Task Consume(ConsumeContext<DownloadVideoMessage> context)
        {
            var operationId = context.Message.Operation.IdOperation;

            _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Processing);

            var downloadedSuccessfully = await _videoDownloader.DownloadVideoNameAndExtensionAsync(context.Message.Url, operationId, context.CancellationToken);
            if (!downloadedSuccessfully)
            {
                _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Error);
                return;
            }

            _operationStatusManager.UpdateOperationProgress(operationId, VideoProcess.Downloading, 10);

            downloadedSuccessfully = await _videoDownloader.DownloadVideoAsync(context.Message.Url, context.Message.OutputFolder, operationId, context.CancellationToken);
            if (!downloadedSuccessfully)
            {
                _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Error);
                return;
            }

            _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Finished);
        }
    }
}