using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Profile.Commands.DeleteProfileCommand
{
    public class DeleteProfileCommand : IRequest<BaseResponse>
    {
        public int IdProfile { get; set; }
    }
}
