using QuizApp.Application.Common.Interfaces;
using QuizApp.Domain.Enums;


namespace QuizApp.Application.Questions.Commands;

public class CreateQuestionCommand : ICommand<Guid>
{
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public int Points { get; set; }
    public int OrderIndex { get; set; }
    public bool IsRequired { get; set; } = true;
    public string? Explanation { get; set; }
    public string? ImageUrl { get; set; }
    public Guid QuizId { get; set; }
}