namespace QuizApp.Application.UserAnswers.DTOs;

public class UpdateUserAnswerDto
{
    public Guid? SelectedAnswerId { get; set; }
    public string? TextAnswer { get; set; }
    public TimeSpan? TimeSpent { get; set; }
}

