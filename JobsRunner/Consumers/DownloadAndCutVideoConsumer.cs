using Application.Interfaces;
using Domain.BusinessEnums;
using Domain.ConsumersContracts;
using JobsRunner.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace JobsRunner.Consumers
{
    public class DownloadAndCutVideoConsumer(IOperationStatusManager _operationStatusManager, IVideoDownloader _videoDownloader, IVideoProcessor _videoProcessor, IMovieCutterDatabase _databaseContext) : IConsumer<DownloadAndCutVideoMessage>
    {
        public async Task Consume(ConsumeContext<DownloadAndCutVideoMessage> context)
        {
            var operationId = context.Message.IdOperation;
            await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Processing);

            // Downloading metadata
            var downloadedSuccessfully = await _videoDownloader.DownloadVideoNameAndExtensionAsync(context.Message.Url, operationId, context.CancellationToken);
            if (!downloadedSuccessfully)
            {
                await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Error);
                return;
            }

            // Downloading video
            downloadedSuccessfully = await _videoDownloader.DownloadVideoAsync(context.Message.Url, context.Message.VideoOutputFolder, operationId, context.CancellationToken);
            if (!downloadedSuccessfully)
            {
                await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Error);
                return;
            }

            // Getting updated video data
            var operation = await _databaseContext.Operations.SingleAsync(p => p.IdOperation == operationId);
            var sourceVideoPath = $"{context.Message.VideoOutputFolder}{operation.VideoName}.{operation.VideoExtension}";
            var processedSuccessfully = true;

            if (context.Message.CutVideoInOnePiece)
            {
                // Cutting whole video into frames
                processedSuccessfully = await _videoProcessor.CutVideoFramesAsync(operationId, sourceVideoPath, context.Message.FramesOutputFolder, operation.VideoName, context.CancellationToken);
            }
            else
            {
                // Cutting video into pieces and frames
                var videoNumber = 1;
                var newVideoPathWithoutExtension = $"{context.Message.VideoOutputFolder}{operation.VideoName}";

                foreach (var piece in context.Message.NewPices)
                {
                    processedSuccessfully = await _videoProcessor.CutVideoPieceAsync(piece, sourceVideoPath, newVideoPathWithoutExtension, operation.VideoExtension, videoNumber, context.CancellationToken);
                    if (!processedSuccessfully)
                    {
                        await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Error);
                        break;
                    }

                    var newVideoPath = $"{newVideoPathWithoutExtension}_{videoNumber}.{operation.VideoExtension}";
                    var newVideoName = $"{operation.VideoName}_{videoNumber}";
                    var newOutputFolder = $"{context.Message.FramesOutputFolder}{newVideoName}";
                    processedSuccessfully = await _videoProcessor.CutVideoFramesAsync(operationId, newVideoPath, newOutputFolder, newVideoName, context.CancellationToken, false);
                    if (!processedSuccessfully)
                    {
                        await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Error);
                        break;
                    }

                    var progress = (int)Math.Round(videoNumber * 100.0 / context.Message.NewPices.Count());
                    await _operationStatusManager.UpdateOperationProgress(operationId, VideoProcess.CuttingIntoPieces, progress);

                    videoNumber++;
                }
            }

            if (processedSuccessfully) await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Finished);
        }
    }
}
