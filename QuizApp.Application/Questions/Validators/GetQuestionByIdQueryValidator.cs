using FluentValidation;
using QuizApp.Application.Questions.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Application.Questions.Validators;

public class GetQuestionByIdQueryValidator : AbstractValidator<GetQuestionByIdQuery>
{
    public GetQuestionByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Question ID is required");
    }
}