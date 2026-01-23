using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Source.Queries.GetSourceLastVideosQuery
{
    public class GetSourceLastVideosQuery : IRequest<BaseResponse>
    {
        public required int IdSource { get; set; }
    }
}
