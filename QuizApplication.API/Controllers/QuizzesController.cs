using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.API.Models.Common;
using QuizApplication.BLL.DTOs;
using QuizApplication.BLL.Interfaces;
using QuizApplication.BLL.Services;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;

namespace QuizApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizzesController> _logger;

        public QuizzesController(
            IQuizService quizService,
            ILogger<QuizzesController> logger)
        {
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all available quizzes with optional filtering
        /// </summary>
        /// <param name="difficulty">Optional filter by quiz difficulty</param>
        /// <param name="category">Optional filter by category ID</param>
        /// <param name="pageNumber">Page number for pagination</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>A paged list of quizzes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Quiz>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResponse<Quiz>>> GetQuizzes(
            [FromQuery] QuizDifficulty? difficulty = null,
            [FromQuery] int? category = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var quizzes = await _quizService.GetAllAsync(cancellationToken);

                // Apply filters if provided
                var filteredQuizzes = quizzes
                    .Where(q => !difficulty.HasValue || q.Difficulty == difficulty)
                    .Where(q => !category.HasValue || q.Categories.Any(c => c.Id == category))
                    .ToList();

                // Apply pagination
                var pagedQuizzes = filteredQuizzes
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new PagedResponse<Quiz>
                {
                    Items = pagedQuizzes,
                    TotalCount = filteredQuizzes.Count,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes");
                return StatusCode(500, new ErrorResponse("An error occurred while retrieving quizzes"));
            }
        }

        /// <summary>
        /// Retrieves a specific quiz by ID
        /// </summary>
        /// <param name="id">Quiz ID</param>
        /// <returns>Quiz details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Quiz), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Quiz>> GetQuiz(int id, CancellationToken cancellationToken)
        {
            try
            {
                var quiz = await _quizService.GetByIdAsync(id, cancellationToken);
                if (quiz == null)
                    return NotFound();

                return Ok(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quiz {QuizId}", id);
                return StatusCode(500, new ErrorResponse($"An error occurred while retrieving quiz {id}"));
            }
        }

        /// <summary>
        /// Retrieves all currently active quizzes
        /// </summary>
        /// <returns>List of active quizzes</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<Quiz>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetActiveQuizzes(CancellationToken cancellationToken)
        {
            try
            {
                var quizzes = await _quizService.GetActiveQuizzesAsync(cancellationToken);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active quizzes");
                return StatusCode(500, new ErrorResponse("An error occurred while retrieving active quizzes"));
            }
        }

        /// <summary>
        /// Retrieves quizzes created by a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of quizzes created by the user</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Quiz>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetUserQuizzes(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var quizzes = await _quizService.GetQuizzesByUserAsync(userId, cancellationToken);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes for user {UserId}", userId);
                return StatusCode(500, new ErrorResponse($"An error occurred while retrieving quizzes for user {userId}"));
            }
        }

        /// <summary>
        /// Retrieves statistics for a specific quiz
        /// </summary>
        /// <param name="id">Quiz ID</param>
        /// <returns>Quiz statistics</returns>
        [HttpGet("{id:int}/statistics")]
        [ProducesResponseType(typeof(QuizStatistics), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizStatistics>> GetQuizStatistics(int id, CancellationToken cancellationToken)
        {
            try
            {
                var statistics = await _quizService.GetQuizStatisticsAsync(id, cancellationToken);
                return Ok(statistics);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for quiz {QuizId}", id);
                return StatusCode(500, new ErrorResponse($"An error occurred while retrieving statistics for quiz {id}"));
            }
        }

        /// <summary>
        /// Creates a new quiz
        /// </summary>
        /// <param name="quiz">Quiz details</param>
        /// <returns>Created quiz</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Quiz), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Quiz>> CreateQuiz([FromBody] Quiz quiz, CancellationToken cancellationToken)
        {
            try
            {
                var createdQuiz = await _quizService.CreateAsync(quiz, cancellationToken);
                return CreatedAtAction(nameof(GetQuiz), new { id = createdQuiz.Id }, createdQuiz);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz");
                return StatusCode(500, new ErrorResponse("An error occurred while creating the quiz"));
            }
        }
        /// <summary>
        /// Updates an existing quiz
        /// </summary>
        /// <param name="id">Quiz identifier</param>
        /// <param name="quiz">Updated quiz details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Quiz updated successfully</response>
        /// <response code="400">Invalid quiz data</response>
        /// <response code="404">Quiz not found</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateQuiz(
            int id,
            Quiz quiz,
            CancellationToken cancellationToken)
        {
            try
            {
                if (id != quiz.Id)
                    return BadRequest(new ErrorResponse("Quiz ID mismatch"));

                await _quizService.UpdateAsync(quiz, cancellationToken);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for quiz update");
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Quiz not found");
                return NotFound(new ErrorResponse(ex.Message));
            }
        }
        /// <summary>
        /// Deletes a quiz
        /// </summary>
        /// <param name="id">Quiz identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Quiz deleted successfully</response>
        /// <response code="404">Quiz not found</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteQuiz(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                await _quizService.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Quiz not found");
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete published quiz");
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Publishes a quiz
        /// </summary>
        /// <param name="id">Quiz identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Published quiz details</returns>
        /// <response code="200">Quiz published successfully</response>
        /// <response code="400">Quiz cannot be published</response>
        /// <response code="404">Quiz not found</response>
        [HttpPatch("{id}/publish")]
        [Authorize]
        [ProducesResponseType(typeof(Quiz), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PublishQuiz(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var quiz = await _quizService.PublishQuizAsync(id, cancellationToken);
                return Ok(quiz);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Quiz validation failed for publishing");
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Quiz not found");
                return NotFound(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Unpublishes a quiz
        /// </summary>
        /// <param name="id">Quiz identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Unpublished quiz details</returns>
        /// <response code="200">Quiz unpublished successfully</response>
        /// <response code="400">Quiz cannot be unpublished</response>
        /// <response code="404">Quiz not found</response>
        [HttpPatch("{id}/unpublish")]
        [Authorize]
        [ProducesResponseType(typeof(Quiz), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnpublishQuiz(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var quiz = await _quizService.UnpublishQuizAsync(id, cancellationToken);
                return Ok(quiz);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Quiz validation failed for unpublishing");
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Quiz not found");
                return NotFound(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves quiz settings
        /// </summary>
        /// <param name="id">Quiz identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Quiz settings</returns>
        /// <response code="200">Quiz settings retrieved successfully</response>
        /// <response code="404">Quiz not found</response>
        [HttpGet("{id}/settings")]
        [ProducesResponseType(typeof(QuizSettings), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuizSettings(
            int id,
            CancellationToken cancellationToken)
        {
            var quiz = await _quizService.GetByIdAsync(id, cancellationToken);
            if (quiz == null)
                return NotFound(new ErrorResponse($"Quiz with ID {id} not found"));

            return Ok(quiz.Settings);
        }
        /// <summary>
        /// Updates quiz settings
        /// </summary>
        /// <param name="id">Quiz identifier</param>
        /// <param name="settings">Updated settings</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Quiz settings updated successfully</response>
        /// <response code="400">Invalid settings data</response>
        /// <response code="404">Quiz not found</response>
        [HttpPut("{id}/settings")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateQuizSettings(
            int id,
            QuizSettings settings,
            CancellationToken cancellationToken)
        {
            try
            {
                var quiz = await _quizService.GetByIdAsync(id, cancellationToken);
                if (quiz == null)
                    return NotFound(new ErrorResponse($"Quiz with ID {id} not found"));

                if (id != settings.QuizId)
                    return BadRequest(new ErrorResponse("Quiz ID mismatch"));

                quiz.Settings = settings;
                await _quizService.UpdateAsync(quiz, cancellationToken);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for quiz settings update");
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }
    }
}
