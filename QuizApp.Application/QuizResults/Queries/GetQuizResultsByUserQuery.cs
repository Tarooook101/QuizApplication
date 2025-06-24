using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizResults.DTOs;


namespace QuizApp.Application.QuizResults.Queries;

public class GetQuizResultsByUserQuery : IQuery<PaginatedResult<QuizResultSummaryDto>>
{
    public Guid UserId { get; set; }
    public PaginationParameters Pagination { get; set; } = new();

    public GetQuizResultsByUserQuery(Guid userId)
    {
        UserId = userId;
    }
}