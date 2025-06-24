using FluentValidation;
using QuizApp.Application.QuizAttempts.Commands;


namespace QuizApp.Application.QuizAttempts.Validators;

public class UpdateQuizAttemptProgressValidator : AbstractValidator<UpdateQuizAttemptProgressCommand>
{
    public UpdateQuizAttemptProgressValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Quiz attempt ID is required");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters");
    }
}