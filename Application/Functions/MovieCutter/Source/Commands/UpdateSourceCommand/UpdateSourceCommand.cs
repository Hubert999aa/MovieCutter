using Application.Mediator;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Source.Commands.UpdateSourceCommand
{
    public class UpdateSourceCommand : IRequest<BaseResponse>
    {
        public required int IdSource { get; set; }
        public required string Name { get; set; } = string.Empty;
        public required string BaseUrl { get; set; } = string.Empty;
        public required int SourceType { get; set; }
        public required int IdProfile { get; set; }
    }
}
