using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.API.Controllersp;
using QuizApp.Application.Auth.Commands;
using QuizApp.Application.Auth.DTOs;
using QuizApp.Application.Auth.Queries;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IMapper _mapper;

    public AuthController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto request)
    {
        var command = _mapper.Map<RegisterCommand>(request);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto request)
    {
        var command = _mapper.Map<LoginCommand>(request);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var query = new GetCurrentUserQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}