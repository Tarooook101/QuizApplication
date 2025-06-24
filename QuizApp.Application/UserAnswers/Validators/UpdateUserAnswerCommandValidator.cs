using FluentValidation;
using QuizApp.Application.UserAnswers.Commands;


namespace QuizApp.Application.UserAnswers.Validators;

public class UpdateUserAnswerCommandValidator : AbstractValidator<UpdateUserAnswerCommand>
{
    public UpdateUserAnswerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User answer ID is required");

        RuleFor(x => x.TextAnswer)
            .MaximumLength(2000)
            .WithMessage("Text answer cannot exceed 2000 characters");

        RuleFor(x => x.TimeSpent)
            .Must(timeSpent => !timeSpent.HasValue || timeSpent.Value >= TimeSpan.Zero)
            .WithMessage("Time spent cannot be negative");
    }
}

