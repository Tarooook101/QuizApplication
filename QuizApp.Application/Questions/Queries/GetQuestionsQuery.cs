using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Questions.DTOs;
using QuizApp.Domain.Enums;


namespace QuizApp.Application.Questions.Queries;

public class GetQuestionsQuery : IQuery<PaginatedResult<QuestionDto>>
{
    public PaginationParameters Pagination { get; set; } = new();
    public Guid? QuizId { get; set; }
    public QuestionType? Type { get; set; }
    public bool? IsRequired { get; set; }
    public string? SearchTerm { get; set; }
}

