using MyMediator.Interfaces;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.VideoProcessing.Requests.CutVideoIntoFramesRequest
{
    public class CutVideoIntoFramesRequest : IRequest<BaseResponse>
    {
        public required string SourceVideoFullPath { get; set; } 
    }
}
