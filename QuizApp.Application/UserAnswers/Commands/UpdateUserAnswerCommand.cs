using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.UserAnswers.DTOs;


namespace QuizApp.Application.UserAnswers.Commands;

public class UpdateUserAnswerCommand : ICommand<UserAnswerDto>
{
    public Guid Id { get; set; }
    public Guid? SelectedAnswerId { get; set; }
    public string? TextAnswer { get; set; }
    public TimeSpan? TimeSpent { get; set; }
}