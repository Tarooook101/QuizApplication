using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.API.Models.Achievements;
using QuizApplication.API.Models.Common;
using QuizApplication.BLL.Interfaces;
using QuizApplication.BLL.Services;

namespace QuizApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementService _achievementService;
        private readonly ILogger<AchievementsController> _logger;

        public AchievementsController(
            IAchievementService achievementService,
            ILogger<AchievementsController> logger)
        {
            _achievementService = achievementService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AchievementResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAchievements(CancellationToken cancellationToken)
        {
            try
            {
                var achievements = await _achievementService.GetAllAsync(cancellationToken);
                var response = achievements.Select(AchievementResponse.FromEntity).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all achievements");
                return StatusCode(500, new ErrorResponse("An error occurred while retrieving achievements"));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AchievementResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAchievement(int id, CancellationToken cancellationToken)
        {
            try
            {
                var achievement = await _achievementService.GetByIdAsync(id, cancellationToken);
                if (achievement == null)
                    return NotFound(new ErrorResponse($"Achievement with ID {id} not found"));

                return Ok(AchievementResponse.FromEntity(achievement));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting achievement {AchievementId}", id);
                return StatusCode(500, new ErrorResponse("An error occurred while retrieving the achievement"));
            }
        }

        [HttpGet("available/{userId}")]
        [ProducesResponseType(typeof(List<AchievementResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableAchievements(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var achievements = await _achievementService.GetAvailableAchievementsAsync(userId, cancellationToken);
                var response = achievements.Select(AchievementResponse.FromEntity).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting available achievements for user {UserId}", userId);
                return StatusCode(500, new ErrorResponse("An error occurred while retrieving available achievements"));
            }
        }

        //[HttpGet("users/{userId}")]
        //[ProducesResponseType(typeof(List<UserAchievementResponse>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetUserAchievements(string userId, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var userAchievements = await _achievementService.GetUserAchievementsAsync(userId, cancellationToken);
        //        var response = userAchievements.Select(UserAchievementResponse.FromEntity).ToList();
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while getting achievements for user {UserId}", userId);
        //        return StatusCode(500, new ErrorResponse("An error occurred while retrieving user achievements"));
        //    }
        //}

        [HttpPost]
        [ProducesResponseType(typeof(AchievementResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAchievement(
            [FromBody] CreateAchievementRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var achievement = request.ToEntity();
                var createdAchievement = await _achievementService.CreateAsync(achievement, cancellationToken);
                var response = AchievementResponse.FromEntity(createdAchievement);

                return CreatedAtAction(
                    nameof(GetAchievement),
                    new { id = response.Id },
                    response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating achievement");
                return StatusCode(500, new ErrorResponse("An error occurred while creating the achievement"));
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAchievement(
            int id,
            [FromBody] UpdateAchievementRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var achievement = await _achievementService.GetByIdAsync(id, cancellationToken);
                if (achievement == null)
                    return NotFound(new ErrorResponse($"Achievement with ID {id} not found"));

                request.UpdateEntity(achievement);
                await _achievementService.UpdateAsync(achievement, cancellationToken);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating achievement {AchievementId}", id);
                return StatusCode(500, new ErrorResponse("An error occurred while updating the achievement"));
            }
        }

        [HttpPost("users/{userId}/award/{achievementId}")]
        [ProducesResponseType(typeof(UserAchievementResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AwardAchievement(
            string userId,
            int achievementId,
            CancellationToken cancellationToken)
        {
            try
            {
                // Check eligibility first
                var isEligible = await _achievementService.CheckUserEligibilityAsync(userId, achievementId, cancellationToken);
                if (!isEligible)
                    return BadRequest(new ErrorResponse("User is not eligible for this achievement"));

                var userAchievement = await _achievementService.AwardAchievementAsync(userId, achievementId, cancellationToken);
                return Ok(UserAchievementResponse.FromEntity(userAchievement));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while awarding achievement {AchievementId} to user {UserId}",
                    achievementId, userId);
                return StatusCode(500, new ErrorResponse("An error occurred while awarding the achievement"));
            }
        }
    }
}
