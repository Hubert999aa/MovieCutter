using Application.Mediator;
using Domain.ConsumersContracts;
using Domain.TechnicalModels;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Application.Functions.MovieCutter.VideoProcessingRequests.DownloadVideoRequest
{
    public class DownloadVideoRequestHandler(IPublishEndpoint _publishEndpoint, IOptions<FolderPathsOptions> _folderPathsOptions) : IRequestHandler<DownloadVideoRequest, BaseResponse>
    {
        public async Task<BaseResponse> Handle(DownloadVideoRequest request, CancellationToken cancellationToken)
        {
            var message = new DownloadVideoMessage
            {
                Url = request.Url,
                OutputFolder = _folderPathsOptions.Value.DownloadFolderPath,
            };

            await _publishEndpoint.Publish(message, cancellationToken);

            return new BaseResponse();
        }
    }
}
