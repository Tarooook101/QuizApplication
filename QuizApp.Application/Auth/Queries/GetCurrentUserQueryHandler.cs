using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using QuizApp.Application.Auth.DTOs;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Auth.Queries;

public class GetCurrentUserQueryHandler : BaseHandler, IQueryHandler<GetCurrentUserQuery, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService,
        IMapper mapper) : base(unitOfWork)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
        {
            return Result.Failure<UserDto>("User is not authenticated");
        }

        var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
        if (user == null || !user.IsActive)
        {
            return Result.Failure<UserDto>("User not found");
        }

        var userDto = _mapper.Map<UserDto>(user);
        return Result.Success(userDto);
    }
}