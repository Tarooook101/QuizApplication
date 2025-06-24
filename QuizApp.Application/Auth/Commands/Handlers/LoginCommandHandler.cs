using Microsoft.AspNetCore.Identity;
using QuizApp.Application.Auth.Commands;
using QuizApp.Application.Auth.DTOs;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Auth.Commands.Handlers;

public class LoginCommandHandler : BaseHandler, ICommandHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService) : base(unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            return Result.Failure<AuthResponseDto>("Invalid email or password");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Result.Failure<AuthResponseDto>("Invalid email or password");
        }

        user.UpdateLastLogin();
        await _userManager.UpdateAsync(user);

        var token = await _tokenService.GenerateTokenAsync(user);

        var response = new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        return Result.Success(response);
    }
}