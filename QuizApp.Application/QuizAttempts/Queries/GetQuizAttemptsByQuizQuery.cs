using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizAttempts.DTOs;


namespace QuizApp.Application.QuizAttempts.Queries;

public class GetQuizAttemptsByQuizQuery : IQuery<PaginatedResult<QuizAttemptDto>>
{
    public Guid QuizId { get; set; }
    public PaginationParameters Pagination { get; set; } = new();
}