using Application.Helpers;
using Application.Mediator;
using Domain.ConsumersContracts;
using Domain.TechnicalEnums;
using Domain.TechnicalModels;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Application.Functions.MovieCutter.VideoProcessingRequests.CutVideoIntoFramesRequest
{
    public class CutVideoIntoFramesRequestHandler(IPublishEndpoint _publishEndpoint, IOptions<FolderPathsOptions> _folderPathsOptions) : IRequestHandler<CutVideoIntoFramesRequest, BaseResponse>
    {
        public async Task<BaseResponse> Handle(CutVideoIntoFramesRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.SourceVideoFullPath)) return new BaseResponse(false, ResponseStatus.ValidationError, "SourceVideoFullPath cannot be empty");

            var videoName = FileNamer.GetVideoName(request.SourceVideoFullPath);
            var message = new CutVideoIntoFramesMessage
            {
                SourceVideoPath = request.SourceVideoFullPath,
                VideoName = videoName,
                OutputFolder = _folderPathsOptions.Value.DownloadFolderPath + videoName,
            };

            await _publishEndpoint.Publish(message, cancellationToken);

            return new BaseResponse();
        }
    }
}
