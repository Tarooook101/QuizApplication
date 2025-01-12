using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.API.Models.Category;
using QuizApplication.BLL.Interfaces;
using QuizApplication.BLL.Services;
using QuizApplication.DAL.Entities;
using System.Net.Mime;

namespace QuizApplication.API.Controllers
{

    /// <summary>
    /// Controller for managing categories in the Quiz Application
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ICategoryService categoryService,
            ILogger<CategoryController> logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all root categories
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of root categories</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Category>>> GetRootCategories(
            CancellationToken cancellationToken)
        {
            try
            {
                var categories = await _categoryService.GetRootCategoriesAsync(cancellationToken);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving root categories");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving categories");
            }
        }

        /// <summary>
        /// Retrieves a specific category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Category details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> GetCategory(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id, cancellationToken);
                return Ok(category);
            }
            catch (NotFoundException)
            {
                return NotFound($"Category with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category {CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving the category");
            }
        }

        /// <summary>
        /// Retrieves subcategories of a specific category
        /// </summary>
        /// <param name="id">Parent category ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of subcategories</returns>
        [HttpGet("{id:int}/subcategories")]
        [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Category>>> GetSubcategories(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                // Verify parent category exists
                await _categoryService.GetByIdAsync(id, cancellationToken);
                var subcategories = await _categoryService.GetSubcategoriesAsync(id, cancellationToken);
                return Ok(subcategories);
            }
            catch (NotFoundException)
            {
                return NotFound($"Parent category with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subcategories for category {CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving subcategories");
            }
        }

        /// <summary>
        /// Retrieves quizzes within a specific category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of quizzes in the category</returns>
        [HttpGet("{id:int}/quizzes")]
        [ProducesResponseType(typeof(IEnumerable<Quiz>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                // Verify category exists
                await _categoryService.GetByIdAsync(id, cancellationToken);
                var quizzes = await _categoryService.GetQuizzesByCategoryAsync(id, cancellationToken);
                return Ok(quizzes);
            }
            catch (NotFoundException)
            {
                return NotFound($"Category with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes for category {CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving quizzes");
            }
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="categoryRequest">Category creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created category</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator,ContentCreator")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Category>> CreateCategory(
            [FromBody] CreateCategoryRequest categoryRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                var category = new Category
                {
                    Name = categoryRequest.Name,
                    Description = categoryRequest.Description,
                    IconUrl = categoryRequest.IconUrl
                };

                var createdCategory = await _categoryService.CreateCategoryWithParentAsync(
                    category,
                    categoryRequest.ParentCategoryId,
                    cancellationToken);

                return CreatedAtAction(
                    nameof(GetCategory),
                    new { id = createdCategory.Id },
                    createdCategory);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException)
            {
                return BadRequest("Specified parent category not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the category");
            }
        }

        /// <summary>
        /// Updates a specific category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="categoryRequest">Category update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,ContentCreator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] UpdateCategoryRequest categoryRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                var existingCategory = await _categoryService.GetByIdAsync(id, cancellationToken);

                existingCategory.Name = categoryRequest.Name;
                existingCategory.Description = categoryRequest.Description;
                existingCategory.IconUrl = categoryRequest.IconUrl;

                await _categoryService.UpdateAsync(existingCategory, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound($"Category with ID {id} not found");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while updating the category");
            }
        }

        /// <summary>
        /// Deletes a specific category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                await _categoryService.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound($"Category with ID {id} not found");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while deleting the category");
            }
        }
    }
}
