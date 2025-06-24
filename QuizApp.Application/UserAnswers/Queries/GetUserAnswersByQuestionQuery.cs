using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.UserAnswers.DTOs;


namespace QuizApp.Application.UserAnswers.Queries;

public class GetUserAnswersByQuestionQuery : IQuery<IEnumerable<UserAnswerDto>>
{
    public Guid QuestionId { get; set; }
}