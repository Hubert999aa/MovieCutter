using Application.Mediator;
using Domain.ConsumersContracts;
using Domain.TechnicalModels;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Application.Functions.MovieCutter.Source.Queries.DownloadSelectedVideoQuery
{
    public class DownloadSelectedVideoQueryHandler(IPublishEndpoint _publishEndpoint, IOptions<FolderPathsOptions> _folderPathsOptions) : IRequestHandler<DownloadSelectedVideoQuery, BaseResponse>
    {
        public async Task<BaseResponse> Handle(DownloadSelectedVideoQuery request, CancellationToken cancellationToken)
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
