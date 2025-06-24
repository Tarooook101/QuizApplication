using QuizApp.Application.Auth.DTOs;
using QuizApp.Application.Common.Interfaces;

namespace QuizApp.Application.Auth.Commands;

public class RegisterCommand : ICommand<AuthResponseDto>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}