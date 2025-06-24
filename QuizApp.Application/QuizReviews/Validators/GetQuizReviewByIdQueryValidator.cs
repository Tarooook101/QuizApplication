using FluentValidation;
using QuizApp.Application.QuizReviews.Queries;


namespace QuizApp.Application.QuizReviews.Validators;

public class GetQuizReviewByIdQueryValidator : AbstractValidator<GetQuizReviewByIdQuery>
{
    public GetQuizReviewByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Review ID is required");
    }
}
