using QuizApp.Application.Common.Interfaces;
using QuizApp.Domain.Enums;


namespace QuizApp.Application.Quizzes.Commands;

public class CreateQuizCommand : ICommand<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int TimeLimit { get; set; }
    public int MaxAttempts { get; set; }
    public QuizDifficulty Difficulty { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? ThumbnailUrl { get; set; }
    public string? Tags { get; set; }
}