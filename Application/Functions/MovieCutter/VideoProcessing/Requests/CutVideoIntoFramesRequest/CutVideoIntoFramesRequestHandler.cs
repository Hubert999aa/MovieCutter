using Application.Helpers;
using MyMediator.Interfaces;
using Domain.ConsumersContracts;
using CoreModels.TechnicalEnums;
using CoreModels.TechnicalModels;
using MassTransit;
using Microsoft.Extensions.Options;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.CutVideoIntoFramesRequest
{
    public class CutVideoIntoFramesRequestHandler(IPublishEndpoint _publishEndpoint, IOptions<FolderPathsOptions> _folderPathsOptions) : IRequestHandler<CutVideoIntoFramesRequest, BaseResponse>
    {
        public async Task<BaseResponse> Handle(CutVideoIntoFramesRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.SourceVideoFullPath)) return new BaseResponse(false, ResponseStatus.ValidationError, "SourceVideoFullPath cannot be empty");

            var videoName = FileNamer.GetFileNameWithoutExtension(request.SourceVideoFullPath);
            var message = new CutVideoIntoFramesMessage
            {
                SourceVideoPath = request.SourceVideoFullPath,
                VideoName = videoName,
                OutputFolder = _folderPathsOptions.Value.FramesFolderPath + videoName + "\\",
            };

            await _publishEndpoint.Publish(message, cancellationToken);

            return new BaseResponse();
        }
    }
}
