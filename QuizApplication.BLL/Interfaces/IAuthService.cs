using QuizApplication.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
        Task<bool> RevokeTokenAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> ValidateEmailConfirmationTokenAsync(string userId, string token, CancellationToken cancellationToken = default);
        Task ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);
    }
}
