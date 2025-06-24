using FluentValidation;
using QuizApp.Application.Answers.Commands;

namespace QuizApp.Application.Answers.Validators;

public class UpdateAnswerCommandValidator : AbstractValidator<UpdateAnswerCommand>
{
    public UpdateAnswerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Answer ID is required");

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Answer text is required")
            .MaximumLength(500)
            .WithMessage("Answer text cannot exceed 500 characters");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Order index cannot be negative");

        RuleFor(x => x.Explanation)
            .MaximumLength(1000)
            .WithMessage("Explanation cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Explanation));
    }
}
