using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Quizzes.DTOs;

namespace QuizApp.Application.Quizzes.Queries;

public class GetQuizByIdQuery : IQuery<QuizDto>
{
    public Guid Id { get; set; }

    public GetQuizByIdQuery(Guid id)
    {
        Id = id;
    }
}