using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.VideoProcessingRequests.CutVideoIntoFramesRequest
{
    public class CutVideoIntoFramesRequest : IRequest<BaseResponse>
    {
        public required string SourceVideoFullPath { get; set; } 
    }
}
