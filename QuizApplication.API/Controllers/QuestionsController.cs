using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.API.Models.Common;
using QuizApplication.API.Models.Option;
using QuizApplication.API.Models.Question;
using QuizApplication.BLL.DTOs;
using QuizApplication.BLL.Interfaces;
using QuizApplication.BLL.Services;
using QuizApplication.DAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

using QuestionResponse = QuizApplication.API.Models.Question.QuestionResponse;
using ValidationException = QuizApplication.BLL.Services.ValidationException;

namespace QuizApplication.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    [Produces("application/json")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(
            IQuestionService questionService,
            ILogger<QuestionController> logger)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all questions for a specific quiz
        /// </summary>
        /// <param name="quizId">The ID of the quiz</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the list of questions</response>
        /// <response code="404">If the quiz is not found</response>
        [HttpGet("quizzes/{quizId}/questions")]
        [ProducesResponseType(typeof(IEnumerable<QuestionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetQuizQuestions(
            [Required] int quizId,
            CancellationToken cancellationToken)
        {
            try
            {
                var questions = await _questionService.GetQuestionsByQuizAsync(quizId, cancellationToken);
                return Ok(questions.Select(q => QuestionResponse.FromEntity(q)));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting questions for quiz {QuizId}", quizId);
                return StatusCode(500, new ErrorResponse("An error occurred while retrieving questions"));
            }
        }

        /// <summary>
        /// Gets a specific question by ID
        /// </summary>
        /// <param name="id">The ID of the question</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the question details</response>
        /// <response code="404">If the question is not found</response>
        [HttpGet("questions/{id}")]
        [ProducesResponseType(typeof(QuestionDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionDetailResponse>> GetQuestion(
            [Required] int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var question = await _questionService.GetByIdAsync(id, cancellationToken);
                if (question == null)
                    return NotFound(new ErrorResponse($"Question with ID {id} not found"));

                return Ok(QuestionDetailResponse.FromEntity(question));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting question {QuestionId}", id);
                return StatusCode(500, new ErrorResponse("An error occurred while retrieving the question"));
            }
        }

        /// <summary>
        /// Gets statistics for a specific question
        /// </summary>
        /// <param name="id">The ID of the question</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the question statistics</response>
        /// <response code="404">If the question is not found</response>
        [HttpGet("questions/{id}/statistics")]
        [ProducesResponseType(typeof(QuestionStatistics), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionStatistics>> GetQuestionStatistics(
            [Required] int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var statistics = await _questionService.GetQuestionStatisticsAsync(id, cancellationToken);
                return Ok(statistics);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for question {QuestionId}", id);
                return StatusCode(500, new ErrorResponse("An error occurred while retrieving question statistics"));
            }
        }

        /// <summary>
        /// Adds a new question to a quiz
        /// </summary>
        /// <param name="quizId">The ID of the quiz</param>
        /// <param name="request">The question creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="201">Returns the newly created question</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If the quiz is not found</response>
        [HttpPost("quizzes/{quizId}/questions")]
        [ProducesResponseType(typeof(QuestionDetailResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionDetailResponse>> CreateQuestion(
            [Required] int quizId,
            [Required] CreateQuestionRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var question = request.ToEntity();
                var createdQuestion = await _questionService.AddQuestionToQuizAsync(quizId, question, cancellationToken);

                return CreatedAtAction(
                    nameof(GetQuestion),
                    new { id = createdQuestion.Id },
                    QuestionDetailResponse.FromEntity(createdQuestion));
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
                _logger.LogError(ex, "Error creating question for quiz {QuizId}", quizId);
                return StatusCode(500, new ErrorResponse("An error occurred while creating the question"));
            }
        }

        /// <summary>
        /// Updates a specific question
        /// </summary>
        /// <param name="id">The ID of the question</param>
        /// <param name="request">The question update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the updated question</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If the question is not found</response>
        [HttpPut("questions/{id}")]
        [ProducesResponseType(typeof(QuestionDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionDetailResponse>> UpdateQuestion(
            [Required] int id,
            [Required] UpdateQuestionRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var question = request.ToEntity(id);
                await _questionService.UpdateAsync(question, cancellationToken);

                var updatedQuestion = await _questionService.GetByIdAsync(id, cancellationToken);
                return Ok(QuestionDetailResponse.FromEntity(updatedQuestion!));
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question {QuestionId}", id);
                return StatusCode(500, new ErrorResponse("An error occurred while updating the question"));
            }
        }

        /// <summary>
        /// Deletes a specific question
        /// </summary>
        /// <param name="id">The ID of the question to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">If the question was successfully deleted</response>
        /// <response code="404">If the question is not found</response>
        [HttpDelete("questions/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteQuestion(
            [Required] int id,
            CancellationToken cancellationToken)
        {
            try
            {
                await _questionService.DeleteAsync(id, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question {QuestionId}", id);
                return StatusCode(500, new ErrorResponse("An error occurred while deleting the question"));
            }
        }

        /// <summary>
        /// Reorders questions in a quiz
        /// </summary>
        /// <param name="quizId">The ID of the quiz</param>
        /// <param name="request">The question reorder request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">If the questions were successfully reordered</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If the quiz is not found</response>
        [HttpPut("quizzes/{quizId}/questions/reorder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReorderQuestions(
            [Required] int quizId,
            [Required] ReorderQuestionsRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _questionService.UpdateQuestionOrderAsync(quizId, request.QuestionOrders, cancellationToken);
                return Ok();
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
                _logger.LogError(ex, "Error reordering questions for quiz {QuizId}", quizId);
                return StatusCode(500, new ErrorResponse("An error occurred while reordering the questions"));
            }
        }

        /// <summary>
        /// Gets options for a specific question
        /// </summary>
        /// <param name="id">The ID of the question</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the list of options</response>
        /// <response code="404">If the question is not found</response>
        [HttpGet("questions/{id}/options")]
        [ProducesResponseType(typeof(IEnumerable<OptionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OptionResponse>>> GetQuestionOptions(
            [Required] int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var question = await _questionService.GetByIdAsync(id, cancellationToken);
                if (question == null)
                    return NotFound(new ErrorResponse($"Question with ID {id} not found"));

                return Ok(question.Options.Select(o => OptionResponse.FromEntity(o)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting options for question {QuestionId}", id);
                return StatusCode(500, new ErrorResponse("An error occurred while retrieving question options"));
            }
        }
    }
}
