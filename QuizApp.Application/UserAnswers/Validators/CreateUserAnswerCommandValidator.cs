using FluentValidation;
using QuizApp.Application.UserAnswers.Commands;


namespace QuizApp.Application.UserAnswers.Validators;

public class CreateUserAnswerCommandValidator : AbstractValidator<CreateUserAnswerCommand>
{
    public CreateUserAnswerCommandValidator()
    {
        RuleFor(x => x.QuizAttemptId)
            .NotEmpty()
            .WithMessage("Quiz attempt ID is required");

        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");

        RuleFor(x => x.TextAnswer)
            .MaximumLength(2000)
            .WithMessage("Text answer cannot exceed 2000 characters");

        RuleFor(x => x.TimeSpent)
            .Must(timeSpent => !timeSpent.HasValue || timeSpent.Value >= TimeSpan.Zero)
            .WithMessage("Time spent cannot be negative");

        RuleFor(x => x)
            .Must(x => x.SelectedAnswerId.HasValue || !string.IsNullOrEmpty(x.TextAnswer))
            .WithMessage("Either a selected answer or text answer must be provided");
    }
}