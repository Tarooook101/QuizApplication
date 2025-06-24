using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizReviews.DTOs;


namespace QuizApp.Application.QuizReviews.Queries;

public class GetQuizReviewsQuery : IQuery<PaginatedResult<QuizReviewDto>>
{
    public Guid? QuizId { get; set; }
    public Guid? UserId { get; set; }
    public bool? IsPublic { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public PaginationParameters Pagination { get; set; } = new();
}