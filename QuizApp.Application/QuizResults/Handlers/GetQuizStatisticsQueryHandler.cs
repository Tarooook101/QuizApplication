using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizResults.DTOs;
using QuizApp.Application.QuizResults.Queries;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.QuizResults.Handlers;

public class GetQuizStatisticsQueryHandler : IQueryHandler<GetQuizStatisticsQuery, QuizStatisticsDto>
{
    private readonly IQuizResultRepository _quizResultRepository;

    public GetQuizStatisticsQueryHandler(IQuizResultRepository quizResultRepository)
    {
        _quizResultRepository = quizResultRepository;
    }

    public async Task<Result<QuizStatisticsDto>> Handle(GetQuizStatisticsQuery request, CancellationToken cancellationToken)
    {
        var results = await _quizResultRepository.GetByQuizIdAsync(request.QuizId, cancellationToken);
        var resultsList = results.ToList();

        if (!resultsList.Any())
        {
            return Result.Success(new QuizStatisticsDto
            {
                QuizId = request.QuizId,
                TotalAttempts = 0
            });
        }

        var passedCount = resultsList.Count(r => r.IsPassed);
        var failedCount = resultsList.Count - passedCount;

        var statistics = new QuizStatisticsDto
        {
            QuizId = request.QuizId,
            TotalAttempts = resultsList.Count,
            AverageScore = resultsList.Average(r => r.Score),
            AveragePercentage = resultsList.Average(r => r.Percentage),
            PassedCount = passedCount,
            FailedCount = failedCount,
            PassRate = resultsList.Count > 0 ? (double)passedCount / resultsList.Count * 100 : 0,
            HighestScore = resultsList.Max(r => r.Score),
            LowestScore = resultsList.Min(r => r.Score),
            AverageTimeSpent = TimeSpan.FromMilliseconds(resultsList.Average(r => r.TimeSpent.TotalMilliseconds))
        };

        return Result.Success(statistics);
    }
}