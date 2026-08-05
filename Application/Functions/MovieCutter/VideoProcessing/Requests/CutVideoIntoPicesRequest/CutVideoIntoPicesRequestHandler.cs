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

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.CutVideoIntoPicesRequest
{
    public class CutVideoIntoPicesRequestHandler(IPublishEndpoint _publishEndpoint, IOptions<FolderPathsOptions> _folderPathsOptions, IMovieCutterDatabase _databaseContext) : IRequestHandler<CutVideoIntoPicesRequest, BaseResponse>
    {
        public async Task<BaseResponse> Handle(CutVideoIntoPicesRequest request, CancellationToken cancellationToken)
        {
            var validationResponse = this.ValidateRequest(request);
            if (!validationResponse.Success) return validationResponse;

            var operation = new Domain.BusinessModels.DatabaseModels.Operation
            {
                OperationType = OperationType.CuttingIntoPiecesOnly,
                VideoProcess = VideoProcess.CuttingIntoPieces,
                VideoName = FileNamer.GetFileNameWithoutExtension(request.SourceVideoFullPath),
                VideoExtension = FileNamer.GetFileExtension(request.SourceVideoFullPath)
            };

            await _databaseContext.Operations.AddAsync(operation);
            await _databaseContext.SaveChangesAsync();

            var message = new CutVideoIntoPicesMessage
            {
                SourceVideoPath = request.SourceVideoFullPath,
                DownloadFolderPath = _folderPathsOptions.Value.DownloadFolderPath,
                NewPieces = request.VideoPices,
                Operation = operation
            };

            await _publishEndpoint.Publish(message, cancellationToken);

            return new BaseResponse();
        }

        public BaseResponse ValidateRequest(CutVideoIntoPicesRequest request)
        {
            bool videoPicesValid = true;

            request.VideoPices.ForEach(p =>
            {
                if (string.IsNullOrEmpty(p.StartTime) || string.IsNullOrEmpty(p.EndTime)) videoPicesValid = false;

                var startTimeValid = TimeSpan.TryParseExact(p.StartTime, @"hh\:mm\:ss", null, out _);
                var endTimeValid = TimeSpan.TryParseExact(p.EndTime, @"hh\:mm\:ss", null, out _);
                if (!startTimeValid || !endTimeValid) videoPicesValid = false;
            });

            if (string.IsNullOrEmpty(request.SourceVideoFullPath)) return new BaseResponse(false, ResponseStatus.ValidationError, "SourceVideoFullPath cannot be empty");
            if (request.VideoPices.Count == 0 || !videoPicesValid) return new BaseResponse(false, ResponseStatus.ValidationError, "VideoPices must be defined properly");

            return new BaseResponse();
        }
    }
}
