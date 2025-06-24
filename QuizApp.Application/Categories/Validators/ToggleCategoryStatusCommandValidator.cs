using FluentValidation;
using QuizApp.Application.Categories.Commands;


namespace QuizApp.Application.Categories.Validators;

public class ToggleCategoryStatusCommandValidator : AbstractValidator<ToggleCategoryStatusCommand>
{
    public ToggleCategoryStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required");
    }
}