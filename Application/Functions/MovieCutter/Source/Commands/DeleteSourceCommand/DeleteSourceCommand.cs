using MyMediator.Interfaces;
using CoreModels.TechnicalModels;

namespace Application.Functions.MovieCutter.Source.Commands.DeleteSourceCommand
{
    public class DeleteSourceCommand : IRequest<BaseResponse>
    {
        public required int IdSource { get; set; }
    }
}
