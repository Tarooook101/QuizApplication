using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Specifications;
using QuizApp.Domain.Specifications.Category;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class CategoryRepository : Repository<Category, Guid>, ICategoryRepository
{
    public CategoryRepository(QuizDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var specification = new ActiveCategoriesSpecification();
        return await GetAsync(specification, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetCategoriesOrderedAsync(CancellationToken cancellationToken = default)
    {
        var specification = new CategoriesOrderedByDisplayOrderSpecification();
        return await GetAsync(specification, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(c => c.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var specification = new CategoryByNameSpecification(name);
        var categories = await GetAsync(specification, cancellationToken);
        return categories.FirstOrDefault();
    }

    
}