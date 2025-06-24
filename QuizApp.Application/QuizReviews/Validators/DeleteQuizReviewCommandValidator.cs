using FluentValidation;
using QuizApp.Application.QuizReviews.Commands;

namespace QuizApp.Application.QuizReviews.Validators;

public class DeleteQuizReviewCommandValidator : AbstractValidator<DeleteQuizReviewCommand>
{
    public DeleteQuizReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Review ID is required");
    }
}