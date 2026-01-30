using FluentValidation;

namespace Application.Functions.MovieCutter.Profile.Commands.CreateProfileCommand
{
    public class CreateProfileCommandValidator : AbstractValidator<CreateProfileCommand>
    {
        public CreateProfileCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotNull()
                .WithMessage("Name is required.")
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name length can't be longer then 100 characters.");
        }
    }
}
