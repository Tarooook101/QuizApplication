using QuizApp.Application.Quizzes.Queries;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Specifications;
using System.Linq.Expressions;

namespace QuizApp.Application.Quizzes.Specifications;

public class GetQuizzesCountSpecification : BaseSpecification<Quiz>
{
    public GetQuizzesCountSpecification(GetQuizzesQuery query) : base(BuildCriteria(query))
    {
        // No paging needed for count
    }

    private static Expression<Func<Quiz, bool>> BuildCriteria(GetQuizzesQuery query)
    {
        return quiz =>
            (query.Difficulty == null || quiz.Difficulty == query.Difficulty) &&
            (query.IsPublic == null || quiz.IsPublic == query.IsPublic) &&
            (query.IsActive == null || quiz.IsActive == query.IsActive) &&
            (query.CreatedByUserId == null || quiz.CreatedByUserId == query.CreatedByUserId) &&
            (string.IsNullOrWhiteSpace(query.SearchTerm) ||
             quiz.Title.ToLower().Contains(query.SearchTerm.ToLower()) ||
             quiz.Description.ToLower().Contains(query.SearchTerm.ToLower()) ||
             (quiz.Tags != null && quiz.Tags.ToLower().Contains(query.SearchTerm.ToLower())));
    }
}