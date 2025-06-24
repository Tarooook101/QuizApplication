namespace QuizApp.Application.QuizAttempts.DTOs;

public class CompleteQuizAttemptDto
{
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public string? Notes { get; set; }
}
