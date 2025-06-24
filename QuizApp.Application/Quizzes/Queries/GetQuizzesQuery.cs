using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Quizzes.DTOs;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.Quizzes.Queries;

public class GetQuizzesQuery : IQuery<PaginatedResult<QuizDto>>
{
    public PaginationParameters Pagination { get; set; } = new();
    public QuizDifficulty? Difficulty { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsActive { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string? SearchTerm { get; set; }
}