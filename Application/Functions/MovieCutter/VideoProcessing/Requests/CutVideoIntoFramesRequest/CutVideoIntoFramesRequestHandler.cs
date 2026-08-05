using Application.Helpers;
using Application.Interfaces;
using CoreModels.TechnicalEnums;
using CoreModels.TechnicalModels;
using Domain.BusinessEnums;
using Domain.ConsumersContracts;
using Domain.TechnicalModels;
using MassTransit;
using Microsoft.Extensions.Options;
using MyMediator.Interfaces;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.CutVideoIntoFramesRequest
{
    public class CutVideoIntoFramesRequestHandler(IPublishEndpoint _publishEndpoint, IOptions<FolderPathsOptions> _folderPathsOptions, IMovieCutterDatabase _databaseContext) : IRequestHandler<CutVideoIntoFramesRequest, BaseResponse>
    {
        public async Task<BaseResponse> Handle(CutVideoIntoFramesRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.SourceVideoFullPath)) return new BaseResponse(false, ResponseStatus.ValidationError, "SourceVideoFullPath cannot be empty");

            var operation = new Domain.BusinessModels.DatabaseModels.Operation
            {
                OperationType = OperationType.CuttingIntoFramesOnly,
                VideoProcess = VideoProcess.CuttingIntoFrames,
                VideoName = FileNamer.GetFileNameWithoutExtension(request.SourceVideoFullPath),
                VideoExtension = FileNamer.GetFileExtension(request.SourceVideoFullPath)
            };

            await _databaseContext.Operations.AddAsync(operation);
            await _databaseContext.SaveChangesAsync();

            var message = new CutVideoIntoFramesMessage
            {
                SourceVideoPath = request.SourceVideoFullPath,
                FramesFolderPath = _folderPathsOptions.Value.FramesFolderPath,
                Operation = operation
            };

            await _publishEndpoint.Publish(message, cancellationToken);

            return new BaseResponse();
        }
    }
}
