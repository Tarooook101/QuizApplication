using FluentValidation;
using QuizApp.Application.Questions.Commands;


namespace QuizApp.Application.Questions.Validators;

public class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
{
    public DeleteQuestionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Question ID is required");
    }
}