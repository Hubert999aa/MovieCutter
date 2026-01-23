using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Source.Queries.DownloadSelectedVideoQuery
{
    public class DownloadSelectedVideoQuery : IRequest<BaseResponse>
    {
        public required string Url { get; set; }
    }
}
