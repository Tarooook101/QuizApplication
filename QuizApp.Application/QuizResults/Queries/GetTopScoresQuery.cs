using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.QuizResults.DTOs;


namespace QuizApp.Application.QuizResults.Queries;

public class GetTopScoresQuery : IQuery<IEnumerable<QuizResultDetailDto>>
{
    public Guid QuizId { get; set; }
    public int Count { get; set; } = 10;

    public GetTopScoresQuery(Guid quizId, int count = 10)
    {
        QuizId = quizId;
        Count = count;
    }
}