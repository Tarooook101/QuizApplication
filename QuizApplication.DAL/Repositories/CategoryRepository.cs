using Microsoft.EntityFrameworkCore;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Database;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Repositories
{
    public class CategoryRepository : Repository<Category, int>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => c.ParentCategoryId == null)
                .Include(c => c.Subcategories)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetSubcategoriesAsync(int parentId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => c.ParentCategoryId == parentId)
                .Include(c => c.Subcategories)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Quiz>> GetQuizzesByCategoryAsync(
            int categoryId,
            bool includeSubcategories = true,
            CancellationToken cancellationToken = default)
        {
            var category = await _dbSet
                .Include(c => c.Quizzes)
                .Include(c => c.Subcategories)
                .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);

            if (category == null)
                return new List<Quiz>();

            var quizzes = new HashSet<Quiz>(category.Quizzes);

            if (includeSubcategories)
            {
                await LoadSubcategoryQuizzesRecursively(category, quizzes, cancellationToken);
            }

            return quizzes
                .Where(q => q.Status == QuizStatus.Published)
                .OrderByDescending(q => q.CreatedAt)
                .ToList();
        }

        private async Task LoadSubcategoryQuizzesRecursively(
            Category category,
            HashSet<Quiz> quizzes,
            CancellationToken cancellationToken)
        {
            foreach (var subcategory in category.Subcategories)
            {
                var subcategoryWithQuizzes = await _dbSet
                    .Include(c => c.Quizzes)
                    .Include(c => c.Subcategories)
                    .FirstOrDefaultAsync(c => c.Id == subcategory.Id, cancellationToken);

                if (subcategoryWithQuizzes != null)
                {
                    foreach (var quiz in subcategoryWithQuizzes.Quizzes)
                    {
                        quizzes.Add(quiz);
                    }
                    await LoadSubcategoryQuizzesRecursively(subcategoryWithQuizzes, quizzes, cancellationToken);
                }
            }
        }

        public override async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.ParentCategory)
                .Include(c => c.Subcategories)
                .Include(c => c.Quizzes)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
    }
}
