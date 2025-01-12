using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.BLL.DTOs;
using QuizApplication.BLL.Interfaces;

namespace QuizApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Register(
            [FromBody] RegisterRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.RegisterAsync(request, cancellationToken);
                return CreatedAtAction(nameof(Register), response);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Registration failed for email: {Email}", request.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.LoginAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Login failed for email: {Email}", request.Email);
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> RefreshToken(
            [FromBody] RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Token refresh failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail(
            [FromQuery] string userId,
            [FromQuery] string token,
            CancellationToken cancellationToken)
        {
            var result = await _authService.ValidateEmailConfirmationTokenAsync(userId, token, cancellationToken);
            return result ? Ok(new { message = "Email confirmed successfully" })
                        : BadRequest(new { message = "Invalid token or user ID" });
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] string email,
            CancellationToken cancellationToken)
        {
            var result = await _authService.ForgotPasswordAsync(email, cancellationToken);
            return result ? Ok(new { message = "Password reset email sent successfully" })
                        : BadRequest(new { message = "Invalid email address" });
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(
            [FromQuery] string email,
            [FromQuery] string token,
            [FromBody] string newPassword,
            CancellationToken cancellationToken)
        {
            var result = await _authService.ResetPasswordAsync(email, token, newPassword, cancellationToken);
            return result ? Ok(new { message = "Password reset successfully" })
                        : BadRequest(new { message = "Invalid token or email" });
        }

        [HttpPost("resend-confirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ResendConfirmationEmail(
            [FromBody] string email,
            CancellationToken cancellationToken)
        {
            await _authService.ResendConfirmationEmailAsync(email, cancellationToken);
            return Ok(new { message = "Confirmation email sent successfully" });
        }
    }
}
