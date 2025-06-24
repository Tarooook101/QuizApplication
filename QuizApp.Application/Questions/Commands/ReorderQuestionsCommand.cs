using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Questions.DTOs;


namespace QuizApp.Application.Questions.Commands;

public class ReorderQuestionsCommand : ICommand
{
    public Guid QuizId { get; set; }
    public List<QuestionOrderDto> Questions { get; set; } = new();
}