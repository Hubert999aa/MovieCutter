using Application.Helpers;
using Application.Mediator;
using Domain.ConsumersContracts;
using Domain.TechnicalEnums;
using Domain.TechnicalModels;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.CutVideoIntoPicesRequest
{
    public class CutVideoIntoPicesRequestHandler(IPublishEndpoint _publishEndpoint, IOptions<FolderPathsOptions> _folderPathsOptions) : IRequestHandler<CutVideoIntoPicesRequest, BaseResponse>
    {
        public async Task<BaseResponse> Handle(CutVideoIntoPicesRequest request, CancellationToken cancellationToken)
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


            var message = new CutVideoIntoPicesMessage
            {
                SourceVideoPath = request.SourceVideoFullPath,
                NewVideoPathWithoutExtension = _folderPathsOptions.Value.DownloadFolderPath + FileNamer.GetFileNameWithoutExtension(request.SourceVideoFullPath),
                NewVideoExtension = FileNamer.GetFileExtension(request.SourceVideoFullPath),
                NewPices = request.VideoPices
            };

            await _publishEndpoint.Publish(message, cancellationToken);

            return new BaseResponse();
        }
    }
}
