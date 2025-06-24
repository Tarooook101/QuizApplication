using FluentValidation;
using QuizApp.Application.QuizResults.Commands;


namespace QuizApp.Application.QuizResults.Validators;

public class UpdateQuizResultFeedbackCommandValidator : AbstractValidator<UpdateQuizResultFeedbackCommand>
{
    public UpdateQuizResultFeedbackCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Quiz result ID is required");

        RuleFor(x => x.Feedback)
            .NotEmpty()
            .WithMessage("Feedback is required")
            .MaximumLength(2000)
            .WithMessage("Feedback cannot exceed 2000 characters");
    }
}
