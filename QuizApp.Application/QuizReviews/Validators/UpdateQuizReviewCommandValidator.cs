using FluentValidation;
using QuizApp.Application.QuizReviews.Commands;

namespace QuizApp.Application.QuizReviews.Validators;

public class UpdateQuizReviewCommandValidator : AbstractValidator<UpdateQuizReviewCommand>
{
    public UpdateQuizReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Review ID is required");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Comment cannot exceed 1000 characters");
    }
}