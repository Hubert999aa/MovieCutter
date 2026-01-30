using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Source.Commands.DeleteSourceCommand
{
    public class DeleteSourceCommand : IRequest<BaseResponse>
    {
        public required int IdSource { get; set; }
    }
}
