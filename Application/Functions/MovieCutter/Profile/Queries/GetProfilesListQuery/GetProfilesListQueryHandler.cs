using Application.Interfaces;
using MyMediator.Interfaces;
using CoreModels.TechnicalModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Functions.MovieCutter.Profile.Queries.GetProfilesListQuery
{
    public class GetProfilesListQueryHandler(IMovieCutterDatabase _context) : IRequestHandler<GetProfilesListQuery, BaseResponse>
    {
        public async Task<BaseResponse> Handle(GetProfilesListQuery request, CancellationToken cancellationToken)
        {
            var profiles = await _context.Profiles
                .Select(p => new
                {
                    p.IdProfile,
                    p.Name,
                    Sources = p.Sources
                        .Select(x => x.SourceType.ToString())
                        .OrderBy(x => x)
                        .ToList(),
                })
                .ToListAsync();

            return new BaseResponse(profiles);
        }
    }
}
