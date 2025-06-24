using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.QuizResults.DTOs;


namespace QuizApp.Application.QuizResults.Queries;

public class GetQuizResultQuery : IQuery<QuizResultDto>
{
    public Guid Id { get; set; }

    public GetQuizResultQuery(Guid id)
    {
        Id = id;
    }
}