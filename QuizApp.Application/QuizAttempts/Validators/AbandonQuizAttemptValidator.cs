using FluentValidation;
using QuizApp.Application.QuizAttempts.Commands;


namespace QuizApp.Application.QuizAttempts.Validators;

public class AbandonQuizAttemptValidator : AbstractValidator<AbandonQuizAttemptCommand>
{
    public AbandonQuizAttemptValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Quiz attempt ID is required");
    }
}