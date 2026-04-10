using Application.Interfaces;
using CoreModels.TechnicalModels;
using Microsoft.EntityFrameworkCore;
using MyMediator.Interfaces;

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
                    p.BaseUrl,
                    SourceType = p.SourceType.ToString()
                })
                .ToListAsync();

            return new BaseResponse(sources);
        }
    }
}
