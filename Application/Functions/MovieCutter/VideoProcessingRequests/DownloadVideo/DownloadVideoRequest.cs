using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.VideoProcessingRequests.DownloadVideo
{
    public class DownloadVideoRequest : IRequest<BaseResponse>
    {
        public required string Url { get; set; }
    }
}
