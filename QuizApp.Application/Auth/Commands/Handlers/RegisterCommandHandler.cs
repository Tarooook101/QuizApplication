using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using QuizApp.Application.Auth.Commands;
using QuizApp.Application.Auth.DTOs;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Auth.Commands.Handlers;

public class RegisterCommandHandler : BaseHandler, ICommandHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IMapper mapper) : base(unitOfWork)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result.Failure<AuthResponseDto>("Email is already registered");
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AuthResponseDto>($"Registration failed: {errors}");
        }

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
