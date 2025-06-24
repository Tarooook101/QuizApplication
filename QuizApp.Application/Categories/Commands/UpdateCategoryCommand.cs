using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.Categories.Commands;

public record UpdateCategoryCommand : ICommand<CategoryDto>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? IconUrl { get; init; }
    public int DisplayOrder { get; init; }
    public string Color { get; init; } = string.Empty;
}