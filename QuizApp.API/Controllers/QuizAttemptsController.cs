using Microsoft.AspNetCore.Mvc;
using QuizApp.API.Controllersp;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizAttempts.Commands;
using QuizApp.Application.QuizAttempts.DTOs;
using QuizApp.Application.QuizAttempts.Queries;

namespace QuizApp.API.Controllers;


[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class QuizAttemptsController : BaseController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuizAttemptDto>> GetQuizAttempt(Guid id)
    {
        var query = new GetQuizAttemptByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllQuizAttempts([FromQuery] PaginationParameters pagination)
    {
        var query = new GetAllQuizAttemptsQuery { Pagination = pagination };
        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult> GetQuizAttemptsByUser(Guid userId, [FromQuery] PaginationParameters pagination)
    {
        var query = new GetQuizAttemptsByUserQuery { UserId = userId, Pagination = pagination };
        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpGet("quiz/{quizId:guid}")]
    public async Task<ActionResult> GetQuizAttemptsByQuiz(Guid quizId, [FromQuery] PaginationParameters pagination)
    {
        var query = new GetQuizAttemptsByQuizQuery { QuizId = quizId, Pagination = pagination };
        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateQuizAttempt([FromBody] CreateQuizAttemptDto dto)
    {
        var command = new CreateQuizAttemptCommand { QuizId = dto.QuizId };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/complete")]
    public async Task<ActionResult> CompleteQuizAttempt(Guid id, [FromBody] CompleteQuizAttemptDto dto)
    {
        var command = new CompleteQuizAttemptCommand
        {
            Id = id,
            Score = dto.Score,
            MaxScore = dto.MaxScore,
            Notes = dto.Notes
        };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/abandon")]
    public async Task<ActionResult> AbandonQuizAttempt(Guid id)
    {
        var command = new AbandonQuizAttemptCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/progress")]
    public async Task<ActionResult> UpdateQuizAttemptProgress(Guid id, [FromBody] UpdateQuizAttemptProgressDto dto)
    {
        var command = new UpdateQuizAttemptProgressCommand { Id = id, Notes = dto.Notes };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
