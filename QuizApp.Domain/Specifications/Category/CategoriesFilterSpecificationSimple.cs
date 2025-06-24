
namespace QuizApp.Domain.Specifications.Category;

public class CategoriesFilterSpecificationSimple : BaseSpecification<Entities.Category>
{
    public CategoriesFilterSpecificationSimple(bool? isActive = null, string? searchTerm = null)
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
    }
}