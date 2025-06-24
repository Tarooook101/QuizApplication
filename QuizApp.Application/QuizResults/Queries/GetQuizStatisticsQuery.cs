using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.QuizResults.DTOs;


namespace QuizApp.Application.QuizResults.Queries;

public class GetQuizStatisticsQuery : IQuery<QuizStatisticsDto>
{
    public Guid QuizId { get; set; }

    public GetQuizStatisticsQuery(Guid quizId)
    {
        QuizId = quizId;
    }
}