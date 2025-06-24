using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizAttempts.DTOs;


namespace QuizApp.Application.QuizAttempts.Queries;

public class GetAllQuizAttemptsQuery : IQuery<PaginatedResult<QuizAttemptDto>>
{
    public PaginationParameters Pagination { get; set; } = new();
}

