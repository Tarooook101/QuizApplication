using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface ICategoryRepository : IRepository<Category, int>
    {
        Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetSubcategoriesAsync(int parentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Quiz>> GetQuizzesByCategoryAsync(int categoryId, bool includeSubcategories = true, CancellationToken cancellationToken = default);
    }
}
