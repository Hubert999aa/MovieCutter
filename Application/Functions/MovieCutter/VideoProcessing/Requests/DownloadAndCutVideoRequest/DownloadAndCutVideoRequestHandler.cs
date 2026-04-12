using CoreModels.TechnicalEnums;
using CoreModels.TechnicalModels;
using Domain.ConsumersContracts;
using Domain.TechnicalModels;
using MassTransit;
using Microsoft.Extensions.Options;
using MyMediator.Interfaces;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.DownloadAndCutVideoRequest
{
    public class DownloadAndCutVideoRequestHandler(IPublishEndpoint _publishEndpoint, IOptions<FolderPathsOptions> _folderPathsOptions)
        : IRequestHandler<DownloadAndCutVideoRequest, BaseResponse>
    {
        public async Task<BaseResponse> Handle(DownloadAndCutVideoRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Url)) return new BaseResponse(false, ResponseStatus.ValidationError, "Url cannot be empty");

            if (!request.CutVideoInOnePiece)
            {
                bool videoPicesValid = true;
                request.VideoPices.ForEach(p =>
                {
                    if (string.IsNullOrEmpty(p.StartTime) || string.IsNullOrEmpty(p.EndTime)) videoPicesValid = false;

                    var startTimeValid = TimeSpan.TryParseExact(p.StartTime, @"hh\:mm\:ss", null, out _);
                    var endTimeValid = TimeSpan.TryParseExact(p.EndTime, @"hh\:mm\:ss", null, out _);
                    if (!startTimeValid || !endTimeValid) videoPicesValid = false;
                });

                if (request.VideoPices.Count == 0 || !videoPicesValid) return new BaseResponse(false, ResponseStatus.ValidationError, "VideoPices must be defined properly");
            }

            var message = new DownloadAndCutVideoMessage
            {
                Url = request.Url,
                VideoOutputFolder = _folderPathsOptions.Value.DownloadFolderPath,
                FramesOutputFolder = _folderPathsOptions.Value.FramesFolderPath,
                CutVideoInOnePiece = request.CutVideoInOnePiece,
                NewPices = request.VideoPices
            };

            await _publishEndpoint.Publish(message, cancellationToken);

            return new BaseResponse();
        }
    }
}
