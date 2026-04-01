using Application.Interfaces;
using MyMediator.Interfaces;
using Domain.TechnicalModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Functions.MovieCutter.Profile.Commands.UpdateProfileCommand
{
    public class UpdateProfileCommandHandler(IMovieCutterDatabase _context) : IRequestHandler<UpdateProfileCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateProfileCommandValidator(_context);
            var validatorResult = await validator.ValidateAsync(request);
            if (!validatorResult.IsValid) return new BaseResponse(validatorResult);

            var profile = await _context.Profiles
                .SingleAsync(p => p.IdProfile == request.IdProfile);

            if (profile.Name != request.Name) profile.Name = request.Name;

            await _context.SaveChangesAsync();

            return new BaseResponse();
        }
    }
}
