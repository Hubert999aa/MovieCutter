using MyMediator.Interfaces;
using CoreModels.TechnicalModels;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.DownloadVideoRequest
{
    public class DownloadVideoRequest : IRequest<BaseResponse>
    {
        public required string Url { get; set; }
    }
}
