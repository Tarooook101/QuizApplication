using QuizApp.Domain.Entities;
using QuizApp.Domain.Specifications;


namespace QuizApp.Domain.Repositories;

public interface ICategoryRepository : IRepository<Category, Guid>
{
    Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetCategoriesOrderedAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IEnumerable<Category>> GetAsync(ISpecification<Category> specification, CancellationToken cancellationToken = default);

}
