using FluentValidation;

namespace Application.Functions.MovieCutter.Source.Commands.CreateSourceCommand
{
    public class CreateSourceCommandValidator : AbstractValidator<CreateSourceCommand>
    {
        public CreateSourceCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotNull()
                .WithMessage("Name is required.")
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name length can't be longer then 100 characters.");

            RuleFor(p => p.BaseUrl)
                .NotNull()
                .WithMessage("BaseUrl is required.")
                .NotEmpty()
                .WithMessage("BaseUrl is required.")
                .MaximumLength(200)
                .WithMessage("BaseUrl length can't be longer then 200 characters.");

            RuleFor(p => p.SourceType)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Value can't be less than 0.");

            RuleFor(p => p.IdProfile)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Value can't be less than 0.");
        }
    }
}
