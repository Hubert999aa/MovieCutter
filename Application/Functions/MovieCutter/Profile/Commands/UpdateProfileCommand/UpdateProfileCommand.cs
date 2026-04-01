using MyMediator.Interfaces;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Profile.Commands.UpdateProfileCommand
{
    public class UpdateProfileCommand : IRequest<BaseResponse>
    {
        public required int IdProfile { get; set; }
        public required string Name { get; set; }
    }
}
