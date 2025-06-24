using FluentValidation;
using QuizApp.Application.Categories.Commands;


namespace QuizApp.Application.Categories.Validators;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required");
    }
}

