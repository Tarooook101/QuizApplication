namespace QuizApp.Application.Answers.DTOs;

public class UpdateAnswerDto
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
    public string? Explanation { get; set; }
}
