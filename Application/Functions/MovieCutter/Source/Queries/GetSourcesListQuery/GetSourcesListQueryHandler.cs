using Application.Interfaces;
using MyMediator.Interfaces;
using CoreModels.TechnicalModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Functions.MovieCutter.Source.Queries.GetSourcesListQuery
{
    public class GetSourcesListQueryHandler(IMovieCutterDatabase _context) : IRequestHandler<GetSourcesListQuery, BaseResponse>
    {
        public async Task<BaseResponse> Handle(GetSourcesListQuery request, CancellationToken cancellationToken)
        {
            var sources = await _context.Sources
                .Where(p => p.IdProfile == request.IdProfile)
                .Select(p => new
                {
                    p.IdSource,
                    p.Name,
                    p.SourceType
                })
                .ToListAsync();

            return new BaseResponse(sources);
        }
    }
}
