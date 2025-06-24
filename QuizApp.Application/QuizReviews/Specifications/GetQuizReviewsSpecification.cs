using QuizApp.Domain.Specifications;
using System.Linq.Expressions;


namespace QuizApp.Application.QuizReviews.Specifications;

public class GetQuizReviewsSpecification : BaseSpecification<QuizApp.Domain.Entities.QuizReview>
{
    public GetQuizReviewsSpecification(
        Guid? quizId,
        Guid? userId,
        bool? isPublic,
        int? minRating,
        int? maxRating,
        int skip,
        int take) : base(BuildCriteria(quizId, userId, isPublic, minRating, maxRating))
    {
        ApplyOrderByDescending(r => r.CreatedAt);
        ApplyPaging(skip, take);
    }

    private static Expression<Func<QuizApp.Domain.Entities.QuizReview, bool>> BuildCriteria(
        Guid? quizId,
        Guid? userId,
        bool? isPublic,
        int? minRating,
        int? maxRating)
    {
        Expression<Func<QuizApp.Domain.Entities.QuizReview, bool>> criteria = r => true;

        if (quizId.HasValue)
        {
            criteria = CombineAnd(criteria, r => r.QuizId == quizId.Value);
        }

        if (userId.HasValue)
        {
            criteria = CombineAnd(criteria, r => r.UserId == userId.Value);
        }

        if (isPublic.HasValue)
        {
            criteria = CombineAnd(criteria, r => r.IsPublic == isPublic.Value);
        }

        if (minRating.HasValue)
        {
            criteria = CombineAnd(criteria, r => r.Rating >= minRating.Value);
        }

        if (maxRating.HasValue)
        {
            criteria = CombineAnd(criteria, r => r.Rating <= maxRating.Value);
        }

        return criteria;
    }

    // Helper method to combine two expressions with AND
    private static Expression<Func<QuizApp.Domain.Entities.QuizReview, bool>> CombineAnd(
        Expression<Func<QuizApp.Domain.Entities.QuizReview, bool>> expr1,
        Expression<Func<QuizApp.Domain.Entities.QuizReview, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(QuizApp.Domain.Entities.QuizReview));

        var body = Expression.AndAlso(
            Expression.Invoke(expr1, parameter),
            Expression.Invoke(expr2, parameter)
        );

        return Expression.Lambda<Func<QuizApp.Domain.Entities.QuizReview, bool>>(body, parameter);
    }
}
