using MyMediator.Interfaces;
using CoreModels.TechnicalModels;

namespace Application.Functions.MovieCutter.Profile.Commands.CreateProfileCommand
{
    public class CreateProfileCommand : IRequest<BaseResponse>
    {
        public required string Name { get; set; }
    }
}
