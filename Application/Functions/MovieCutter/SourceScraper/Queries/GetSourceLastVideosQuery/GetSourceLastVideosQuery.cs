using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.SourceScraper.Queries.GetSourceLastVideosQuery
{
    public class GetSourceLastVideosQuery : IRequest<BaseResponse>
    {
        public int IdSource { get; set; }
    }
}
