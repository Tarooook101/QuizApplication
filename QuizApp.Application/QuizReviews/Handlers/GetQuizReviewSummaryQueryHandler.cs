using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizReviews.DTOs;
using QuizApp.Application.QuizReviews.Queries;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.QuizReviews.Handlers;

public class GetQuizReviewSummaryQueryHandler : IQueryHandler<GetQuizReviewSummaryQuery, QuizReviewSummaryDto>
{
    private readonly IQuizReviewRepository _quizReviewRepository;

    public GetQuizReviewSummaryQueryHandler(IQuizReviewRepository quizReviewRepository)
    {
        _quizReviewRepository = quizReviewRepository;
    }

    public async Task<Result<QuizReviewSummaryDto>> Handle(GetQuizReviewSummaryQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _quizReviewRepository.GetPublicReviewsByQuizAsync(request.QuizId, cancellationToken);
        var reviewList = reviews.ToList();

        if (!reviewList.Any())
        {
            return Result.Success(new QuizReviewSummaryDto
            {
                QuizId = request.QuizId,
                AverageRating = 0,
                TotalReviews = 0
            });
        }

        var summary = new QuizReviewSummaryDto
        {
            QuizId = request.QuizId,
            AverageRating = Math.Round(reviewList.Average(r => r.Rating), 2),
            TotalReviews = reviewList.Count,
            FiveStarCount = reviewList.Count(r => r.Rating == 5),
            FourStarCount = reviewList.Count(r => r.Rating == 4),
            ThreeStarCount = reviewList.Count(r => r.Rating == 3),
            TwoStarCount = reviewList.Count(r => r.Rating == 2),
            OneStarCount = reviewList.Count(r => r.Rating == 1),
            RecommendedCount = reviewList.Count(r => r.IsRecommended)
        };

        summary.RecommendationPercentage = summary.TotalReviews > 0
            ? Math.Round((double)summary.RecommendedCount / summary.TotalReviews * 100, 2)
            : 0;

        return Result.Success(summary);
    }
}
