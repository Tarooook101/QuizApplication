using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.UserAnswers.DTOs;


namespace QuizApp.Application.UserAnswers.Commands;

public class GradeUserAnswerCommand : ICommand<UserAnswerDto>
{
    public Guid Id { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
}
