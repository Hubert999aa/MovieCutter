using MyMediator.Interfaces;
using CoreModels.TechnicalModels;

namespace Application.Functions.MovieCutter.Profile.Commands.DeleteProfileCommand
{
    public class DeleteProfileCommand : IRequest<BaseResponse>
    {
        public required int IdProfile { get; set; }
    }
}
