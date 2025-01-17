using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.API.Models.Common;
using QuizApplication.BLL.DTOs;
using QuizApplication.BLL.Interfaces;
using QuizApplication.BLL.Services;
using QuizApplication.DAL.Entities;
using System.Security.Claims;

namespace QuizApplication.API.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api")]
    [Produces("application/json")]
    public class QuizAttemptController : ControllerBase
    {
        private readonly IQuizAttemptService _quizAttemptService;
        private readonly ILogger<QuizAttemptController> _logger;

        public QuizAttemptController(
            IQuizAttemptService quizAttemptService,
            ILogger<QuizAttemptController> logger)
        {
            _quizAttemptService = quizAttemptService ?? throw new ArgumentNullException(nameof(quizAttemptService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Starts a new quiz attempt for the authenticated user
        /// </summary>
        /// <param name="quizId">The ID of the quiz to attempt</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">Quiz attempt created successfully</response>
        /// <response code="400">Invalid request or user not eligible for attempt</response>
        /// <response code="404">Quiz not found</response>
        [HttpPost("quizzes/{quizId}/attempts")]
        [ProducesResponseType(typeof(QuizAttempt), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> StartAttempt(
            int quizId,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var attempt = await _quizAttemptService.StartQuizAttemptAsync(userId, quizId, cancellationToken);

                return CreatedAtAction(
                    nameof(GetAttempt),
                    new { attemptId = attempt.Id },
                    attempt);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting quiz attempt for quiz {QuizId}", quizId);
                throw;
            }
        }

        /// <summary>
        /// Submits responses for a quiz attempt
        /// </summary>
        /// <param name="attemptId">The ID of the attempt to submit</param>
        /// <param name="responses">The list of question responses</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Attempt submitted successfully</response>
        /// <response code="400">Invalid request or attempt already submitted</response>
        /// <response code="404">Attempt not found</response>
        [HttpPut("attempts/{attemptId}/submit")]
        [ProducesResponseType(typeof(QuizAttempt), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SubmitAttempt(
            int attemptId,
            [FromBody] IEnumerable<QuestionResponse> responses,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var attempt = await _quizAttemptService.GetByIdAsync(attemptId, cancellationToken);

                if (attempt?.UserId != userId)
                {
                    return Forbid();
                }

                var submittedAttempt = await _quizAttemptService.SubmitQuizAttemptAsync(
                    attemptId,
                    responses,
                    cancellationToken);

                return Ok(submittedAttempt);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting quiz attempt {AttemptId}", attemptId);
                throw;
            }
        }

        /// <summary>
        /// Gets the details of a specific quiz attempt
        /// </summary>
        /// <param name="attemptId">The ID of the attempt to retrieve</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Attempt details retrieved successfully</response>
        /// <response code="404">Attempt not found</response>
        [HttpGet("attempts/{attemptId}")]
        [ProducesResponseType(typeof(QuizAttempt), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAttempt(
            int attemptId,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var attempt = await _quizAttemptService.GetByIdAsync(attemptId, cancellationToken);

                if (attempt == null)
                {
                    return NotFound(new ErrorResponse($"Attempt {attemptId} not found"));
                }

                if (attempt.UserId != userId)
                {
                    return Forbid();
                }

                return Ok(attempt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quiz attempt {AttemptId}", attemptId);
                throw;
            }
        }

        /// <summary>
        /// Gets the results of a completed quiz attempt
        /// </summary>
        /// <param name="attemptId">The ID of the attempt to get results for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Attempt results retrieved successfully</response>
        /// <response code="404">Attempt not found</response>
        [HttpGet("attempts/{attemptId}/results")]
        [ProducesResponseType(typeof(QuizAttemptResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAttemptResults(
            int attemptId,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var attempt = await _quizAttemptService.GetByIdAsync(attemptId, cancellationToken);

                if (attempt == null)
                {
                    return NotFound(new ErrorResponse($"Attempt {attemptId} not found"));
                }

                if (attempt.UserId != userId)
                {
                    return Forbid();
                }

                var results = await _quizAttemptService.GetAttemptResultAsync(attemptId, cancellationToken);
                return Ok(results);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving results for attempt {AttemptId}", attemptId);
                throw;
            }
        }

        /// <summary>
        /// Gets all attempts for a specific user and quiz
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="quizId">The ID of the quiz</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Attempts retrieved successfully</response>
        [HttpGet("users/{userId}/quizzes/{quizId}/attempts")]
        [ProducesResponseType(typeof(IEnumerable<QuizAttempt>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserQuizAttempts(
            string userId,
            int quizId,
            CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != currentUserId)
                {
                    return Forbid();
                }

                var attempts = await _quizAttemptService.GetUserAttemptsAsync(
                    userId,
                    quizId,
                    cancellationToken);

                return Ok(attempts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving attempts for user {UserId} and quiz {QuizId}", userId, quizId);
                throw;
            }
        }

        /// <summary>
        /// Gets all responses for a specific attempt
        /// </summary>
        /// <param name="attemptId">The ID of the attempt</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Responses retrieved successfully</response>
        /// <response code="404">Attempt not found</response>
        [HttpGet("attempts/{attemptId}/responses")]
        [ProducesResponseType(typeof(IEnumerable<QuestionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAttemptResponses(
            int attemptId,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var attempt = await _quizAttemptService.GetByIdAsync(attemptId, cancellationToken);

                if (attempt == null)
                {
                    return NotFound(new ErrorResponse($"Attempt {attemptId} not found"));
                }

                if (attempt.UserId != userId)
                {
                    return Forbid();
                }

                return Ok(attempt.Responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving responses for attempt {AttemptId}", attemptId);
                throw;
            }
        }
    }
}
