

namespace QuizApp.Domain.Specifications.Category;

public class CategoriesOrderedByDisplayOrderSpecification : BaseSpecification<Entities.Category>
{
    public CategoriesOrderedByDisplayOrderSpecification()
    {
        ApplyOrderBy(c => c.DisplayOrder);
    }
}
