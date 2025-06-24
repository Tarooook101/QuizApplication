using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.UserAnswers.DTOs;


namespace QuizApp.Application.UserAnswers.Queries;

public class GetUserAnswerByIdQuery : IQuery<UserAnswerDetailsDto>
{
    public Guid Id { get; set; }
}