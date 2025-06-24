using QuizApp.Application.Questions.Queries;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Specifications;
using System.Linq.Expressions;


namespace QuizApp.Application.Questions.Specifications;

public class GetQuestionsSpecification : BaseSpecification<Question>
{
    public GetQuestionsSpecification(GetQuestionsQuery query)
        : base(BuildCriteria(query))
    {
        AddInclude(q => q.Quiz);
        ApplyPaging(query.Pagination.Skip, query.Pagination.Take);
        ApplyOrderBy(q => q.OrderIndex);
    }

    private static Expression<Func<Question, bool>> BuildCriteria(GetQuestionsQuery query)
    {
        return q =>
            (!query.QuizId.HasValue || q.QuizId == query.QuizId.Value) &&
            (!query.Type.HasValue || q.Type == query.Type.Value) &&
            (!query.IsRequired.HasValue || q.IsRequired == query.IsRequired.Value) &&
            (string.IsNullOrWhiteSpace(query.SearchTerm) ||
                q.Text.ToLower().Contains(query.SearchTerm.ToLower()) ||
                (q.Explanation != null && q.Explanation.ToLower().Contains(query.SearchTerm.ToLower()))
            );
    }
}
