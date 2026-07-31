using CoreModels.TechnicalModels;
using Domain.ConsumersContracts;
using MyMediator.Interfaces;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.DownloadAndCutVideoRequest
{
    public class DownloadAndCutVideoRequest : IRequest<BaseResponse>
    {
        public required string Url { get; set; }
        public required bool CutVideoInOnePiece { get; set; }
        public required List<VideoPiece> VideoPices { get; set; }
    }
}
