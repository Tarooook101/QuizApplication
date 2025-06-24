using FluentValidation;
using QuizApp.Application.QuizAttempts.Commands;


namespace QuizApp.Application.QuizAttempts.Validators;

public class CompleteQuizAttemptValidator : AbstractValidator<CompleteQuizAttemptCommand>
{
    public CompleteQuizAttemptValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Quiz attempt ID is required");

        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Score cannot be negative");

        RuleFor(x => x.MaxScore)
            .GreaterThan(0)
            .WithMessage("Max score must be positive");

        RuleFor(x => x.Score)
            .LessThanOrEqualTo(x => x.MaxScore)
            .WithMessage("Score cannot exceed max score");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters");
    }
}
