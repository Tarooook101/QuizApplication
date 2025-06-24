using QuizApp.Application.Auth.DTOs;
using QuizApp.Application.Common.Interfaces;


namespace QuizApp.Application.Auth.Commands;
public class LoginCommand : ICommand<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}