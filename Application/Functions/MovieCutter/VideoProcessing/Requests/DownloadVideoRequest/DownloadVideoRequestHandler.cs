using MyMediator.Interfaces;
using Domain.ConsumersContracts;
using Domain.TechnicalEnums;
using Domain.TechnicalModels;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.DownloadVideoRequest
{
    public class DownloadVideoRequestHandler(IPublishEndpoint _publishEndpoint, IOptions<FolderPathsOptions> _folderPathsOptions) : IRequestHandler<DownloadVideoRequest, BaseResponse>
    {
        public async Task<BaseResponse> Handle(DownloadVideoRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Url)) return new BaseResponse(false, ResponseStatus.ValidationError, "Url cannot be empty");

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
