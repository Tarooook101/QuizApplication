namespace QuizApp.Application.QuizResults.DTOs;

public class QuizResultDetailDto : QuizResultDto
{
    public string UserName { get; set; } = string.Empty;
    public string QuizTitle { get; set; } = string.Empty;
}