

using System.Linq.Expressions;

namespace QuizApp.Domain.Specifications.Category;

public class CategoriesFilterSpecification : BaseSpecification<Entities.Category>
{
    public CategoriesFilterSpecification(bool? isActive = null, string? searchTerm = null)
    {
        // Build the criteria expression properly
        Expression<Func<Entities.Category, bool>>? criteria = null;

        if (isActive.HasValue)
        {
            criteria = c => c.IsActive == isActive.Value;
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var searchCriteria = BuildSearchCriteria(searchTerm);

            if (criteria != null)
            {
                criteria = CombineExpressions(criteria, searchCriteria);
            }
            else
            {
                criteria = searchCriteria;
            }
        }

        if (criteria != null)
        {
            Criteria = criteria;
        }
    }

    private Expression<Func<Entities.Category, bool>> BuildSearchCriteria(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return c => c.Name.ToLower().Contains(lowerSearchTerm) ||
                   c.Description.ToLower().Contains(lowerSearchTerm);
    }

    private Expression<Func<Entities.Category, bool>> CombineExpressions(
        Expression<Func<Entities.Category, bool>> expr1,
        Expression<Func<Entities.Category, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(Entities.Category), "c");
        var body1 = ReplaceParameter(expr1.Body, expr1.Parameters[0], parameter);
        var body2 = ReplaceParameter(expr2.Body, expr2.Parameters[0], parameter);
        var combinedBody = Expression.AndAlso(body1, body2);

        return Expression.Lambda<Func<Entities.Category, bool>>(combinedBody, parameter);
    }

    private Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}
