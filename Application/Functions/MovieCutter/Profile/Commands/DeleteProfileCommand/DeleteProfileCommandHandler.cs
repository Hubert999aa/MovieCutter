using Application.Interfaces;
using MyMediator.Interfaces;
using CoreModels.TechnicalModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Functions.MovieCutter.Profile.Commands.DeleteProfileCommand
{
    public class DeleteProfileCommandHandler(IMovieCutterDatabase _context) : IRequestHandler<DeleteProfileCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _context.Profiles
                .SingleOrDefaultAsync(p => p.IdProfile == request.IdProfile);

            if (profile != null)
            {
                _context.Profiles.Remove(profile);
                await _context.SaveChangesAsync();
            }

            return new BaseResponse();
        }
    }
}
