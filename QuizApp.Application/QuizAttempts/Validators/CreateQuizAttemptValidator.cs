using FluentValidation;
using QuizApp.Application.QuizAttempts.Commands;

namespace QuizApp.Application.QuizAttempts.Validators;

public class CreateQuizAttemptValidator : AbstractValidator<CreateQuizAttemptCommand>
{
    public CreateQuizAttemptValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("Quiz ID is required");
    }
}