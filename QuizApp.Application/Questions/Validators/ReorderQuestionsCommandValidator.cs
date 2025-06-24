using FluentValidation;
using QuizApp.Application.Questions.Commands;


namespace QuizApp.Application.Questions.Validators;

public class ReorderQuestionsCommandValidator : AbstractValidator<ReorderQuestionsCommand>
{
    public ReorderQuestionsCommandValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty().WithMessage("Quiz ID is required");

        RuleFor(x => x.Questions)
            .NotEmpty().WithMessage("Questions list cannot be empty");

        RuleForEach(x => x.Questions).SetValidator(new QuestionOrderDtoValidator());
    }
}