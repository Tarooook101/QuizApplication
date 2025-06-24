namespace QuizApp.Domain.Specifications.Category;

public class CategoryByNameSpecification : BaseSpecification<Entities.Category>
{
    public CategoryByNameSpecification(string name) : base(c => c.Name.ToLower() == name.ToLower())
    {
    }
}