using FluentValidation;
using QuizApp.Application.QuizReviews.Queries;

namespace QuizApp.Application.QuizReviews.Validators;

public class GetQuizReviewsQueryValidator : AbstractValidator<GetQuizReviewsQuery>
{
    public GetQuizReviewsQueryValidator()
    {
        RuleFor(x => x.MinRating)
            .InclusiveBetween(1, 5)
            .When(x => x.MinRating.HasValue)
            .WithMessage("Minimum rating must be between 1 and 5");

        RuleFor(x => x.MaxRating)
            .InclusiveBetween(1, 5)
            .When(x => x.MaxRating.HasValue)
            .WithMessage("Maximum rating must be between 1 and 5");

        RuleFor(x => x)
            .Must(x => !x.MinRating.HasValue || !x.MaxRating.HasValue || x.MinRating <= x.MaxRating)
            .WithMessage("Minimum rating cannot be greater than maximum rating");
    }
}