using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.API.Models.Tag;
using QuizApplication.BLL.Interfaces;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Interfaces;

namespace QuizApplication.API.Controllers
{

    [ApiController]
    [Route("api/tags")]
    [Produces("application/json")]
    public class TagController : ControllerBase
    {
        private readonly IQuizTagRepository _tagRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<TagController> _logger;
        private const string CacheKeyPrefix = "tag_";

        public TagController(
            IQuizTagRepository tagRepository,
            ICacheService cacheService,
            ILogger<TagController> logger)
        {
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all quiz tags
        /// </summary>
        /// <returns>List of tags</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TagResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TagResponseDto>>> GetAllTags(
            CancellationToken cancellationToken)
        {
            try
            {
                // Try to get from cache first
                var cacheKey = $"{CacheKeyPrefix}all";
                var cachedTags = await _cacheService.GetAsync<List<TagResponseDto>>(cacheKey, cancellationToken);

                if (cachedTags != null)
                {
                    return Ok(cachedTags);
                }

                var tags = await _tagRepository.GetAllAsync(cancellationToken);
                var tagDtos = tags.Select(t => new TagResponseDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description
                }).ToList();

                // Cache the results
                await _cacheService.SetAsync(cacheKey, tagDtos, TimeSpan.FromMinutes(15), cancellationToken);

                return Ok(tagDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all tags");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Gets a specific tag by ID
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns>Tag details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TagResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TagResponseDto>> GetTagById(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"{CacheKeyPrefix}{id}";
                var cachedTag = await _cacheService.GetAsync<TagResponseDto>(cacheKey, cancellationToken);

                if (cachedTag != null)
                {
                    return Ok(cachedTag);
                }

                var tag = await _tagRepository.GetByIdAsync(id, cancellationToken);
                if (tag == null)
                {
                    return NotFound($"Tag with ID {id} not found");
                }

                var tagDto = new TagResponseDto
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Description = tag.Description
                };

                await _cacheService.SetAsync(cacheKey, tagDto, TimeSpan.FromMinutes(15), cancellationToken);

                return Ok(tagDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tag with ID {TagId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Gets all quizzes with a specific tag
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns>List of quizzes</returns>
        [HttpGet("{id:int}/quizzes")]
        [ProducesResponseType(typeof(IEnumerable<QuizSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<QuizSummaryDto>>> GetQuizzesByTag(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"{CacheKeyPrefix}{id}_quizzes";
                var cachedQuizzes = await _cacheService.GetAsync<List<QuizSummaryDto>>(cacheKey, cancellationToken);

                if (cachedQuizzes != null)
                {
                    return Ok(cachedQuizzes);
                }

                var quizzes = await _tagRepository.GetQuizzesByTagAsync(id, cancellationToken);
                if (!quizzes.Any())
                {
                    return NotFound($"No quizzes found for tag with ID {id}");
                }

                var quizDtos = quizzes.Select(q => new QuizSummaryDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Difficulty = q.Difficulty,
                    Status = q.Status,
                    CreatedAt = q.CreatedAt
                }).ToList();

                await _cacheService.SetAsync(cacheKey, quizDtos, TimeSpan.FromMinutes(15), cancellationToken);

                return Ok(quizDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching quizzes for tag with ID {TagId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Creates a new tag
        /// </summary>
        /// <param name="request">Tag creation request</param>
        /// <returns>Created tag details</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator,ContentCreator")]
        [ProducesResponseType(typeof(TagResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TagResponseDto>> CreateTag(
            [FromBody] CreateTagRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tag = new QuizTag
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedBy = User.Identity?.Name ?? "System",
                    CreatedAt = DateTimeOffset.UtcNow
                };

                var createdTag = await _tagRepository.AddAsync(tag, cancellationToken);

                // Invalidate cache
                await _cacheService.RemoveAsync($"{CacheKeyPrefix}all", cancellationToken);

                var response = new TagResponseDto
                {
                    Id = createdTag.Id,
                    Name = createdTag.Name,
                    Description = createdTag.Description
                };

                return CreatedAtAction(nameof(GetTagById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new tag");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Updates an existing tag
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <param name="request">Tag update request</param>
        /// <returns>No content</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,ContentCreator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTag(
            int id,
            [FromBody] UpdateTagRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tag = await _tagRepository.GetByIdAsync(id, cancellationToken);
                if (tag == null)
                {
                    return NotFound($"Tag with ID {id} not found");
                }

                tag.Name = request.Name;
                tag.Description = request.Description;
                tag.LastModifiedBy = User.Identity?.Name ?? "System";
                tag.LastModifiedAt = DateTimeOffset.UtcNow;

                await _tagRepository.UpdateAsync(tag, cancellationToken);

                // Invalidate cache
                await _cacheService.RemoveAsync($"{CacheKeyPrefix}all", cancellationToken);
                await _cacheService.RemoveAsync($"{CacheKeyPrefix}{id}", cancellationToken);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating tag with ID {TagId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Deletes a tag
        /// </summary>
        /// <param name="id">Tag ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTag(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var tag = await _tagRepository.GetByIdAsync(id, cancellationToken);
                if (tag == null)
                {
                    return NotFound($"Tag with ID {id} not found");
                }

                await _tagRepository.DeleteAsync(tag, cancellationToken);

                // Invalidate cache
                await _cacheService.RemoveAsync($"{CacheKeyPrefix}all", cancellationToken);
                await _cacheService.RemoveAsync($"{CacheKeyPrefix}{id}", cancellationToken);
                await _cacheService.RemoveAsync($"{CacheKeyPrefix}{id}_quizzes", cancellationToken);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting tag with ID {TagId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }
    }
}
