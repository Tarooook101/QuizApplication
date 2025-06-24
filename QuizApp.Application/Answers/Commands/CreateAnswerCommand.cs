using QuizApp.Application.Answers.DTOs;
using QuizApp.Application.Common.Interfaces;


namespace QuizApp.Application.Answers.Commands;

public class CreateAnswerCommand : ICommand<AnswerDto>
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
    public string? Explanation { get; set; }
    public Guid QuestionId { get; set; }
}