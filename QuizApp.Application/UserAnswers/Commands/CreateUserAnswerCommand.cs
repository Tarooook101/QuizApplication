using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.UserAnswers.DTOs;

namespace QuizApp.Application.UserAnswers.Commands;

public class CreateUserAnswerCommand : ICommand<UserAnswerDto>
{
    public Guid QuizAttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid? SelectedAnswerId { get; set; }
    public string? TextAnswer { get; set; }
    public TimeSpan? TimeSpent { get; set; }
}