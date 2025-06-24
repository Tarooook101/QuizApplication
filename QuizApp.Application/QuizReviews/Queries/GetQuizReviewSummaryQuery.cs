using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.QuizReviews.DTOs;

namespace QuizApp.Application.QuizReviews.Queries;

public class GetQuizReviewSummaryQuery : IQuery<QuizReviewSummaryDto>
{
    public Guid QuizId { get; set; }
}
