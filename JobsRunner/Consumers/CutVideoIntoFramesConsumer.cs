using Application.Interfaces;
using Domain.BusinessEnums;
using Domain.ConsumersContracts;
using JobsRunner.Interfaces;
using MassTransit;

namespace JobsRunner.Consumers
{
    public class CutVideoIntoFramesConsumer(IOperationStatusManager _operationStatusManager, IVideoProcessor _videoProcessor) : IConsumer<CutVideoIntoFramesMessage>
    {
        public async Task Consume(ConsumeContext<CutVideoIntoFramesMessage> context)
        {
            var operationId = context.Message.Operation.IdOperation;
            await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Processing);

            var outputFolder = context.Message.FramesFolderPath + context.Message.Operation.VideoName + "\\";
            Directory.CreateDirectory(outputFolder);

            var processedSuccessfully = await _videoProcessor.CutVideoFramesAsync(context.Message.Operation.IdOperation, context.Message.SourceVideoPath, outputFolder, context.Message.Operation.VideoName, context.CancellationToken);

            if (processedSuccessfully)
                await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Finished);
            else
                await _operationStatusManager.UpdateOperationStatus(operationId, OperationStatus.Error);
        }
    }
}
