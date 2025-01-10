using Microsoft.Extensions.Logging;
using QuizApplication.BLL.Interfaces;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Services
{

    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CategoryService> _logger;
        private const string CacheKeyPrefix = "Category_";

        public CategoryService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}{id}";
            var cachedCategory = await _cacheService.GetAsync<Category>(cacheKey, cancellationToken);

            if (cachedCategory != null)
            {
                return cachedCategory;
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), id);
            }

            await _cacheService.SetAsync(cacheKey, category, TimeSpan.FromHours(1), cancellationToken);
            return category;
        }

        public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}All";
            var cachedCategories = await _cacheService.GetAsync<IReadOnlyList<Category>>(cacheKey, cancellationToken);

            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
            await _cacheService.SetAsync(cacheKey, categories, TimeSpan.FromHours(1), cancellationToken);
            return categories;
        }

        public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            ValidateCategory(category);

            try
            {
                var createdCategory = await _unitOfWork.Categories.AddAsync(category, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await InvalidateCategoryCache();
                return createdCategory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category: {CategoryName}", category.Name);
                throw new ServiceException("Failed to create category", ex);
            }
        }

        public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            ValidateCategory(category);

            try
            {
                await _unitOfWork.Categories.UpdateAsync(category, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await InvalidateCategoryCache();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category: {CategoryId}", category.Id);
                throw new ServiceException("Failed to update category", ex);
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await GetByIdAsync(id, cancellationToken);

            // Check if category has subcategories
            var hasSubcategories = await _unitOfWork.Categories
                .AnyAsync(c => c.ParentCategoryId == id, cancellationToken);

            if (hasSubcategories)
            {
                throw new ValidationException("Cannot delete category with existing subcategories");
            }

            // Check if category has associated quizzes
            var hasQuizzes = (await GetQuizzesByCategoryAsync(id, cancellationToken)).Any();
            if (hasQuizzes)
            {
                throw new ValidationException("Cannot delete category with associated quizzes");
            }

            try
            {
                await _unitOfWork.Categories.DeleteAsync(category, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await InvalidateCategoryCache();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category: {CategoryId}", id);
                throw new ServiceException("Failed to delete category", ex);
            }
        }

        public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}RootCategories";
            var cachedCategories = await _cacheService.GetAsync<IReadOnlyList<Category>>(cacheKey, cancellationToken);

            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            var rootCategories = await _unitOfWork.Categories.GetRootCategoriesAsync(cancellationToken);
            await _cacheService.SetAsync(cacheKey, rootCategories, TimeSpan.FromHours(1), cancellationToken);
            return rootCategories;
        }

        public async Task<IReadOnlyList<Category>> GetSubcategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}Subcategories_{parentCategoryId}";
            var cachedSubcategories = await _cacheService.GetAsync<IReadOnlyList<Category>>(cacheKey, cancellationToken);

            if (cachedSubcategories != null)
            {
                return cachedSubcategories;
            }

            var subcategories = await _unitOfWork.Categories.GetSubcategoriesAsync(parentCategoryId, cancellationToken);
            await _cacheService.SetAsync(cacheKey, subcategories, TimeSpan.FromHours(1), cancellationToken);
            return subcategories;
        }

        public async Task<IReadOnlyList<Quiz>> GetQuizzesByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}Quizzes_{categoryId}";
            var cachedQuizzes = await _cacheService.GetAsync<IReadOnlyList<Quiz>>(cacheKey, cancellationToken);

            if (cachedQuizzes != null)
            {
                return cachedQuizzes;
            }

            var quizzes = await _unitOfWork.Categories.GetQuizzesByCategoryAsync(categoryId, true, cancellationToken);
            await _cacheService.SetAsync(cacheKey, quizzes, TimeSpan.FromMinutes(30), cancellationToken);
            return quizzes;
        }

        public async Task<Category> CreateCategoryWithParentAsync(Category category, int? parentId, CancellationToken cancellationToken = default)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (parentId.HasValue)
            {
                var parentCategory = await GetByIdAsync(parentId.Value, cancellationToken);
                category.ParentCategoryId = parentId;
            }

            return await CreateAsync(category, cancellationToken);
        }

        private void ValidateCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ValidationException("Category name is required");
            }

            if (category.Name.Length > 100)
            {
                throw new ValidationException("Category name cannot exceed 100 characters");
            }

            if (category.Description?.Length > 500)
            {
                throw new ValidationException("Category description cannot exceed 500 characters");
            }
        }

        private async Task InvalidateCategoryCache()
        {
            try
            {
                await _cacheService.RemoveAsync($"{CacheKeyPrefix}All");
                await _cacheService.RemoveAsync($"{CacheKeyPrefix}RootCategories");
                // Note: Specific category and subcategory caches will be refreshed on next request
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error occurred while invalidating category cache");
                // Don't throw - cache invalidation errors shouldn't break the main operation
            }
        }
    }
}
