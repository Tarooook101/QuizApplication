using QuizApp.Domain.Enums;


namespace QuizApp.Application.Questions.DTOs;

public class QuestionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public int Points { get; set; }
    public int OrderIndex { get; set; }
    public bool IsRequired { get; set; }
    public string? Explanation { get; set; }
    public string? ImageUrl { get; set; }
    public Guid QuizId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
