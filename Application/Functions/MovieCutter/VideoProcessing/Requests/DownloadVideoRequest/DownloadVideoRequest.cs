using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.DownloadVideoRequest
{
    public class DownloadVideoRequest : IRequest<BaseResponse>
    {
        public required string Url { get; set; }
    }
}
