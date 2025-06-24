

namespace QuizApp.Domain.Specifications.Category;

public class CategoriesPaginatedSpecificationSimple : BaseSpecification<Entities.Category>
{
    public CategoriesPaginatedSpecificationSimple(
        int pageNumber,
        int pageSize,
        bool? isActive = null,
        string? searchTerm = null)
    {
        if (isActive.HasValue && !string.IsNullOrEmpty(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            Criteria = c => c.IsActive == isActive.Value &&
                           (c.Name.ToLower().Contains(lowerSearchTerm) ||
                            c.Description.ToLower().Contains(lowerSearchTerm));
        }
        else if (isActive.HasValue)
        {
            Criteria = c => c.IsActive == isActive.Value;
        }
        else if (!string.IsNullOrEmpty(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            Criteria = c => c.Name.ToLower().Contains(lowerSearchTerm) ||
                           c.Description.ToLower().Contains(lowerSearchTerm);
        }

        ApplyOrderBy(c => c.DisplayOrder);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}