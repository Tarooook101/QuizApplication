namespace QuizApp.Application.Answers.DTOs;

public class CreateAnswerDto
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
    public string? Explanation { get; set; }
    public Guid QuestionId { get; set; }
}