using FluentValidation;
using QuizApp.Application.QuizReviews.Commands;

namespace QuizApp.Application.QuizReview.Validators;

public class CreateQuizReviewCommandValidator : AbstractValidator<CreateQuizReviewCommand>
{
    public CreateQuizReviewCommandValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("Quiz ID is required");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Comment cannot exceed 1000 characters");
    }
}