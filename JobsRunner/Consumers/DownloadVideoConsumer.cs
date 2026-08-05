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
            await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Processing);

            var downloadedSuccessfully = await _videoDownloader.DownloadVideoNameAndExtensionAsync(context.Message.Url, operationId, context.CancellationToken);
            if (!downloadedSuccessfully)
            {
                await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Error);
                return;
            }

            await _operationStatusManager.UpdateOperationProgress(operationId, VideoProcess.DownloadingMetadata);

            downloadedSuccessfully = await _videoDownloader.DownloadVideoAsync(context.Message.Url, context.Message.OutputFolder, operationId, context.CancellationToken);
            if (!downloadedSuccessfully)
            {
                await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Error);
                return;
            }

            await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Finished);
        }
    }
}