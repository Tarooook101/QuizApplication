namespace QuizApp.Application.Categories.DTOs;

public record CreateCategoryDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? IconUrl { get; init; }
    public int DisplayOrder { get; init; }
    public string Color { get; init; } = "#6366f1";
}