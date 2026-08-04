using Application.Interfaces;
using CoreModels.TechnicalEnums;
using CoreModels.TechnicalModels;
using Domain.BusinessEnums;
using Domain.ConsumersContracts;
using Domain.TechnicalModels;
using MassTransit;
using Microsoft.Extensions.Options;
using MyMediator.Interfaces;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.DownloadVideoRequest
{
    public class DownloadVideoRequestHandler(IPublishEndpoint _publishEndpoint, IOptions<FolderPathsOptions> _folderPathsOptions, IMovieCutterDatabase _databaseContext) : IRequestHandler<DownloadVideoRequest, BaseResponse>
    {
        public async Task<BaseResponse> Handle(DownloadVideoRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Url)) return new BaseResponse(false, ResponseStatus.ValidationError, "Url cannot be empty");

            var operation = new Domain.BusinessModels.DatabaseModels.Operation
            {
                OperationType = OperationType.DownloadOnly,
                VideoProcess = VideoProcess.Downloading,
                VideoName = request.VideoName,
            };

            await _databaseContext.Operations.AddAsync(operation);
            await _databaseContext.SaveChangesAsync();

            var message = new DownloadVideoMessage
            {
                Url = request.Url,
                OutputFolder = _folderPathsOptions.Value.DownloadFolderPath,
                Operation = operation
            };

            await _publishEndpoint.Publish(message, cancellationToken);

            return new BaseResponse();
        }
    }
}
