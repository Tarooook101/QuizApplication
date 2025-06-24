namespace QuizApp.Domain.Specifications.Category;

public class ActiveCategoriesSpecification : BaseSpecification<Entities.Category>
{
    public ActiveCategoriesSpecification() : base(c => c.IsActive)
    {
        ApplyOrderBy(c => c.DisplayOrder);
    }
}
