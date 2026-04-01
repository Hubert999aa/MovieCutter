using MyMediator.Interfaces;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Profile.Commands.CreateProfileCommand
{
    public class CreateProfileCommand : IRequest<BaseResponse>
    {
        public required string Name { get; set; }
    }
}
