using FluentValidation;
using QuizApp.Application.Questions.DTOs;


namespace QuizApp.Application.Questions.Validators;
public class QuestionOrderDtoValidator : AbstractValidator<QuestionOrderDto>
{
    public QuestionOrderDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Question ID is required");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Order index cannot be negative");
    }
}
