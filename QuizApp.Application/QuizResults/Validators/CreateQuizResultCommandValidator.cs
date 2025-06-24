using FluentValidation;
using QuizApp.Application.QuizResults.Commands;


namespace QuizApp.Application.QuizResults.Validators;

public class CreateQuizResultCommandValidator : AbstractValidator<CreateQuizResultCommand>
{
    public CreateQuizResultCommandValidator()
    {
        RuleFor(x => x.QuizAttemptId)
            .NotEmpty()
            .WithMessage("Quiz attempt ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("Quiz ID is required");

        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Score must be non-negative")
            .LessThanOrEqualTo(x => x.MaxScore)
            .WithMessage("Score cannot exceed max score");

        RuleFor(x => x.MaxScore)
            .GreaterThan(0)
            .WithMessage("Max score must be positive");

        RuleFor(x => x.CorrectAnswers)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Correct answers must be non-negative")
            .LessThanOrEqualTo(x => x.TotalQuestions)
            .WithMessage("Correct answers cannot exceed total questions");

        RuleFor(x => x.TotalQuestions)
            .GreaterThan(0)
            .WithMessage("Total questions must be positive");

        RuleFor(x => x.TimeSpent)
            .GreaterThanOrEqualTo(TimeSpan.Zero)
            .WithMessage("Time spent cannot be negative");

        RuleFor(x => x.PassingThreshold)
            .InclusiveBetween(0.0, 100.0)
            .When(x => x.PassingThreshold.HasValue)
            .WithMessage("Passing threshold must be between 0 and 100");

        RuleFor(x => x.Feedback)
            .MaximumLength(2000)
            .WithMessage("Feedback cannot exceed 2000 characters");
    }
}
