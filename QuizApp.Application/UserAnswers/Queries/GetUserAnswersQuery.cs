using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.UserAnswers.DTOs;


namespace QuizApp.Application.UserAnswers.Queries;

public class GetUserAnswersQuery : IQuery<PaginatedResult<UserAnswerDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? QuizAttemptId { get; set; }
    public Guid? QuestionId { get; set; }
    public bool? IsCorrect { get; set; }
}