using QuizApp.Domain.Entities;

namespace QuizApp.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
    Task<bool> ValidateTokenAsync(string token);
}
