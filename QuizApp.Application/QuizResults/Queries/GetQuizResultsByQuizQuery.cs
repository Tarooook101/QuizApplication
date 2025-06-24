using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizResults.DTOs;


namespace QuizApp.Application.QuizResults.Queries;

public class GetQuizResultsByQuizQuery : IQuery<PaginatedResult<QuizResultDetailDto>>
{
    public Guid QuizId { get; set; }
    public PaginationParameters Pagination { get; set; } = new();

    public GetQuizResultsByQuizQuery(Guid quizId)
    {
        QuizId = quizId;
    }
}
