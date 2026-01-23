using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.VideoProcessing.DownloadVideo
{
    public class DownloadVideoRequest : IRequest<BaseResponse>
    {
        public required string Url { get; set; }
    }
}
