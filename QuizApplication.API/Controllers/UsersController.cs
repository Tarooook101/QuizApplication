using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.API.Models.Common;
using QuizApplication.BLL.DTOs;
using QuizApplication.BLL.Interfaces;
using QuizApplication.BLL.Services;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using ValidationException = QuizApplication.BLL.Services.ValidationException;

namespace QuizApplication.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="id">The user's unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the user details</response>
        /// <response code="404">If the user is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApplicationUser), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplicationUser>> GetUser(
            [Required] string id,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id, cancellationToken);
                return Ok(user);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {UserId}", id);
                return NotFound(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Gets a user's profile
        /// </summary>
        /// <param name="id">The user's unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the user profile</response>
        /// <response code="404">If the user profile is not found</response>
        [HttpGet("{id}/profile")]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserProfile>> GetUserProfile(
            [Required] string id,
            CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _userService.GetUserProfileAsync(id, cancellationToken);
                return Ok(profile);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User profile not found: {UserId}", id);
                return NotFound(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Updates a user's profile
        /// </summary>
        /// <param name="id">The user's unique identifier</param>
        /// <param name="profile">The updated profile information</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Profile updated successfully</response>
        /// <response code="400">If the profile data is invalid</response>
        /// <response code="404">If the user is not found</response>
        [HttpPut("{id}/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateUserProfile(
            [Required] string id,
            [Required] UserProfile profile,
            CancellationToken cancellationToken)
        {
            try
            {
                if (id != profile.UserId)
                {
                    return BadRequest(new ErrorResponse("User ID mismatch"));
                }

                await _userService.UpdateUserProfileAsync(id, profile, cancellationToken);
                return Ok();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Invalid profile data for user: {UserId}", id);
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {UserId}", id);
                return NotFound(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Gets a user's statistics
        /// </summary>
        /// <param name="id">The user's unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the user statistics</response>
        /// <response code="404">If the user is not found</response>
        [HttpGet("{id}/statistics")]
        [ProducesResponseType(typeof(UserStatistics), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 300)] // Cache for 5 minutes
        public async Task<ActionResult<UserStatistics>> GetUserStatistics(
            [Required] string id,
            CancellationToken cancellationToken)
        {
            try
            {
                var statistics = await _userService.GetUserStatisticsAsync(id, cancellationToken);
                return Ok(statistics);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {UserId}", id);
                return NotFound(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Updates a user's notification preferences
        /// </summary>
        /// <param name="id">The user's unique identifier</param>
        /// <param name="preferences">The updated notification preferences</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Preferences updated successfully</response>
        /// <response code="400">If the preferences data is invalid</response>
        /// <response code="404">If the user is not found</response>
        [HttpPut("{id}/notification-preferences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateNotificationPreferences(
            [Required] string id,
            [Required] NotificationPreferences preferences,
            CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _userService.GetUserProfileAsync(id, cancellationToken);
                profile.NotificationPreferences = preferences;
                await _userService.UpdateUserProfileAsync(id, profile, cancellationToken);
                return Ok();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Invalid notification preferences for user: {UserId}", id);
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {UserId}", id);
                return NotFound(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        /// <param name="email">The user's email address</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">Returns the user details</response>
        /// <response code="404">If the user is not found</response>
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(ApplicationUser), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplicationUser>> GetUserByEmail(
            [Required][EmailAddress] string email,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email, cancellationToken);
                return Ok(user);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found with email: {Email}", email);
                return NotFound(new ErrorResponse(ex.Message));
            }
        }
    }
}
