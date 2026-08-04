using Application.Interfaces;
using Domain.BusinessEnums;
using Domain.ConsumersContracts;
using JobsRunner.Interfaces;
using MassTransit;

namespace JobsRunner.Consumers
{
    public class CutVideoIntoPicesConsumer(IOperationStatusManager _operationStatusManager, IVideoProcessor _videoProcessor) : IConsumer<CutVideoIntoPicesMessage>
    {
        public async Task Consume(ConsumeContext<CutVideoIntoPicesMessage> context)
        {
            var operationId = context.Message.Operation.IdOperation;
            await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Processing);

            var videoNumber = 1;
            var processedSuccessfully = true;
            var newVideoPathWithoutExtension = $"{context.Message.DownloadFolderPath}{context.Message.Operation.VideoName}";

            foreach (var piece in context.Message.NewPieces)
            {
                processedSuccessfully = await _videoProcessor.CutVideoPieceAsync(piece, context.Message.SourceVideoPath, newVideoPathWithoutExtension, context.Message.Operation.VideoExtension, videoNumber, context.CancellationToken);
                if (!processedSuccessfully)
                {
                    await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Error);
                    break;
                }

                var progress = (int)Math.Round(videoNumber * 100.0 / context.Message.NewPieces.Count());
                await _operationStatusManager.UpdateOperationProgress(operationId, VideoProcess.CuttingIntoPieces, progress);

                videoNumber++;
            }

            if (processedSuccessfully)
            {
                await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Finished);
            }
        }
    }
}
