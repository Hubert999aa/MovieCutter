using MyMediator.Interfaces;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Source.Commands.CreateSourceCommand
{
    public class CreateSourceCommand : IRequest<BaseResponse>
    {
        public required string Name { get; set; } = string.Empty;
        public required string BaseUrl { get; set; } = string.Empty;
        public required int SourceType { get; set; }
        public required int IdProfile { get; set; }
    }
}
