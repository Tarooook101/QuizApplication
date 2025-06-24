using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.QuizReviews.DTOs;


namespace QuizApp.Application.QuizReviews.Queries;

public class GetQuizReviewByIdQuery : IQuery<QuizReviewDto>
{
    public Guid Id { get; set; }
}