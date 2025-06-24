namespace QuizApp.Application.UserAnswers.DTOs;

public class UserAnswerDetailsDto : UserAnswerDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string? SelectedAnswerText { get; set; }
    public bool IsQuestionRequired { get; set; }
    public int QuestionPoints { get; set; }
}
