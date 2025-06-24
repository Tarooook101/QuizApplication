using FluentValidation;
using QuizApp.Application.UserAnswers.Commands;

namespace QuizApp.Application.UserAnswers.Validators;

public class GradeUserAnswerCommandValidator : AbstractValidator<GradeUserAnswerCommand>
{
    public GradeUserAnswerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User answer ID is required");

        RuleFor(x => x.PointsEarned)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Points earned cannot be negative");
    }
}