using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;


namespace QuizApp.Application.Categories.Queries;

public record GetCategoriesPaginatedQuery : IQuery<PaginatedResult<CategoryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public bool? IsActive { get; init; }
    public string? SearchTerm { get; init; }
}