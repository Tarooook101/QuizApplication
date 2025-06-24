using FluentValidation;
using QuizApp.Application.Questions.Commands;


namespace QuizApp.Application.Questions.Validators;

public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Question text is required")
            .MaximumLength(1000).WithMessage("Question text cannot exceed 1000 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid question type");

        RuleFor(x => x.Points)
            .GreaterThan(0).WithMessage("Points must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Points cannot exceed 100");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(-1).WithMessage("Order index must be -1 or greater (-1 for auto-assignment)");

        RuleFor(x => x.QuizId)
            .NotEmpty().WithMessage("Quiz ID is required");

        RuleFor(x => x.Explanation)
            .MaximumLength(2000).WithMessage("Explanation cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Explanation));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }
}