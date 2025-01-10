using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Interfaces
{
    public interface ICategoryService : IService<Category, int>
    {
        Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetSubcategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Quiz>> GetQuizzesByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<Category> CreateCategoryWithParentAsync(Category category, int? parentId, CancellationToken cancellationToken = default);
    }
}
