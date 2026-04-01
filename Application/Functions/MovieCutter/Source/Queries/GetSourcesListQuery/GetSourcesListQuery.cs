using MyMediator.Interfaces;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Source.Queries.GetSourcesListQuery
{
    public class GetSourcesListQuery : IRequest<BaseResponse>
    {
        public required int IdProfile { get; set; }
    }
}
