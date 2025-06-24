using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.QuizAttempts.DTOs;

namespace QuizApp.Application.QuizAttempts.Queries;

public class GetQuizAttemptByIdQuery : IQuery<QuizAttemptDto>
{
    public Guid Id { get; set; }

    public GetQuizAttemptByIdQuery(Guid id)
    {
        Id = id;
    }
}