namespace QuizApp.Application.Answers.DTOs;

public class AnswerDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
    public string? Explanation { get; set; }
    public Guid QuestionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}