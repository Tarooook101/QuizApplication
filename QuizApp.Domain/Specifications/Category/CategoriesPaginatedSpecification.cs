using System.Linq.Expressions;

namespace QuizApp.Domain.Specifications.Category;

public class CategoriesPaginatedSpecification : BaseSpecification<Entities.Category>
{
    public CategoriesPaginatedSpecification(
        int pageNumber,
        int pageSize,
        bool? isActive = null,
        string? searchTerm = null)
    {
        // Build the criteria expression properly
        Expression<Func<Entities.Category, bool>>? criteria = null;

        if (isActive.HasValue)
        {
            criteria = c => c.IsActive == isActive.Value;
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            var searchCriteria = new Func<Entities.Category, bool>(c =>
                c.Name.ToLower().Contains(lowerSearchTerm) ||
                c.Description.ToLower().Contains(lowerSearchTerm));

            if (criteria != null)
            {
                var existingCriteria = criteria.Compile();
                Criteria = c => existingCriteria(c) && searchCriteria(c);
            }
            else
            {
                Criteria = c => searchCriteria(c);
            }
        }
        else if (criteria != null)
        {
            Criteria = criteria;
        }

        ApplyOrderBy(c => c.DisplayOrder);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}