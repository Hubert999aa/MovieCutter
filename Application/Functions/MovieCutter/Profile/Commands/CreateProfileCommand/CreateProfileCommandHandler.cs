using Application.Interfaces;
using MyMediator.Interfaces;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Profile.Commands.CreateProfileCommand
{
    public class CreateProfileCommandHandler(IMovieCutterDatabase _context) : IRequestHandler<CreateProfileCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateProfileCommandValidator();
            var validatorResult = await validator.ValidateAsync(request);
            if (!validatorResult.IsValid) return new BaseResponse(validatorResult);

            var newProfile = new Domain.BusinessModels.DatabaseModels.Profile
            {
                Name = request.Name,
            };

            await _context.Profiles.AddAsync(newProfile);
            await _context.SaveChangesAsync();

            return new BaseResponse();
        }
    }
}
