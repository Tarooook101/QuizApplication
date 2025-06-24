using FluentValidation;
using QuizApp.Application.QuizReviews.Queries;

namespace QuizApp.Application.QuizReviews.Validators;

public class GetQuizReviewSummaryQueryValidator : AbstractValidator<GetQuizReviewSummaryQuery>
{
    public GetQuizReviewSummaryQueryValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("Quiz ID is required");
    }
}