using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Source.Commands.CreateSourceCommand
{
    public class CreateSourceCommand : IRequest<BaseResponse>
    {
        public string Name { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public int SourceType { get; set; }
        public int IdProfile { get; set; }
    }
}
