using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuizApplication.BLL.DTOs;
using QuizApplication.BLL.Interfaces;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            // Validate if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ApplicationException("User with this email already exists.");
            }

            // Create new user
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                Status = UserStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "System"
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Add to role
            await _userManager.AddToRoleAsync(user, request.Role.ToString());

            // Create user profile
            var profile = new UserProfile
            {
                UserId = user.Id,
                TimeZone = "UTC",
                Language = "en",
                NotificationPreferences = new NotificationPreferences
                {
                    EmailNotifications = true,
                    PushNotifications = false,
                    QuizReminders = true,
                    AchievementNotifications = true
                },
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "System"
            };

            await _unitOfWork.UserProfiles.AddAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate email confirmation token and send email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendConfirmationEmailAsync(user, token);

            // Generate JWT token and refresh token
            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || user.IsDeleted)
            {
                throw new ApplicationException("Invalid credentials.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new ApplicationException("Please confirm your email before logging in.");
            }

            if (user.Status == UserStatus.Suspended)
            {
                throw new ApplicationException("Your account has been suspended.");
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new ApplicationException("Invalid credentials.");
            }

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var principal = GetPrincipalFromExpiredToken(request.RefreshToken);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new ApplicationException("Invalid refresh token.");
            }

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<bool> RevokeTokenAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> ValidateEmailConfirmationTokenAsync(string userId, string token, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.Status = UserStatus.Active;
                await _userManager.UpdateAsync(user);
                return true;
            }

            return false;
        }

        public async Task ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.EmailConfirmed)
            {
                return;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendConfirmationEmailAsync(user, token);
        }

        public async Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.EmailConfirmed)
            {
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await SendPasswordResetEmailAsync(user, token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        private async Task<AuthResponse> GenerateAuthResponseAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenExpiryMinutes = int.Parse(_configuration["Jwt:ExpiryInMinutes"]);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken,
                ExpiresAt = tokenDescriptor.Expires.Value,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    Role = user.Role,
                    Status = user.Status
                }
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private async Task SendConfirmationEmailAsync(ApplicationUser user, string token)
        {
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{_configuration["AppUrl"]}/confirm-email?userId={user.Id}&token={encodedToken}";

            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm your email",
                $"Please confirm your email by clicking this link: {confirmationLink}");
        }

        private async Task SendPasswordResetEmailAsync(ApplicationUser user, string token)
        {
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var resetLink = $"{_configuration["AppUrl"]}/reset-password?email={user.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(
                user.Email,
                "Reset your password",
                $"Please reset your password by clicking this link: {resetLink}");
        }
    }
}
