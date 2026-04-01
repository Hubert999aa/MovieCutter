using MyMediator.Interfaces;
using Domain.ConsumersContracts;
using CoreModels.TechnicalModels;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.CutVideoIntoPicesRequest
{
    public class CutVideoIntoPicesRequest : IRequest<BaseResponse>
    {
        public required string SourceVideoFullPath { get; set; }
        public required List<VideoPice> VideoPices { get; set; }
    }
}
