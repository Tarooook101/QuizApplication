using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Questions.DTOs;


namespace QuizApp.Application.Questions.Queries;

public class GetQuestionByIdQuery : IQuery<QuestionDto>
{
    public Guid Id { get; set; }

    public GetQuestionByIdQuery(Guid id)
    {
        Id = id;
    }
}