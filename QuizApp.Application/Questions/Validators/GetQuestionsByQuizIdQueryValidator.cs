using FluentValidation;
using QuizApp.Application.Questions.Queries;


namespace QuizApp.Application.Questions.Validators;

public class GetQuestionsByQuizIdQueryValidator : AbstractValidator<GetQuestionsByQuizIdQuery>
{
    public GetQuestionsByQuizIdQueryValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty().WithMessage("Quiz ID is required");
    }
}