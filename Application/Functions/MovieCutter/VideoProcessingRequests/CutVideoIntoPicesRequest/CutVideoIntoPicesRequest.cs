using Application.Mediator;
using Domain.ConsumersContracts;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.VideoProcessingRequests.CutVideoIntoPicesRequest
{
    public class CutVideoIntoPicesRequest : IRequest<BaseResponse>
    {
        public required string SourceVideoFullPath { get; set; }
        public required List<VideoPice> VideoPices { get; set; }
    }
}
