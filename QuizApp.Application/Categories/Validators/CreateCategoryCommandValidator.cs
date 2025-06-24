using FluentValidation;
using QuizApp.Application.Categories.Commands;

namespace QuizApp.Application.Categories.Validators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Category name is required and must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Category description is required and must not exceed 500 characters");

        RuleFor(x => x.IconUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.IconUrl))
            .WithMessage("Icon URL must not exceed 500 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order must be a positive number");

        RuleFor(x => x.Color)
            .NotEmpty()
            .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("Color must be a valid hex color code");
    }
}
