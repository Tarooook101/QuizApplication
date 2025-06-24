using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.UserAnswers.DTOs;


namespace QuizApp.Application.UserAnswers.Queries;

public class GetUserAnswersByQuizAttemptQuery : IQuery<IEnumerable<UserAnswerDetailsDto>>
{
    public Guid QuizAttemptId { get; set; }
}