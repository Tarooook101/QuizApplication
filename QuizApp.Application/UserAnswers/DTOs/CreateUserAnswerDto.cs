namespace QuizApp.Application.UserAnswers.DTOs;

public class CreateUserAnswerDto
{
    public Guid QuizAttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid? SelectedAnswerId { get; set; }
    public string? TextAnswer { get; set; }
    public TimeSpan? TimeSpent { get; set; }
}