using QuizApp.Application.Answers.DTOs;
using QuizApp.Application.Common.Interfaces;


namespace QuizApp.Application.Answers.Queries;

public class GetAnswerByIdQuery : IQuery<AnswerDto>
{
    public Guid Id { get; set; }

    public GetAnswerByIdQuery(Guid id)
    {
        Id = id;
    }
}
