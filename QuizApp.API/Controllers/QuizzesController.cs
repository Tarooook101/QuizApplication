using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.API.Controllersp;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Quizzes.Commands;
using QuizApp.Application.Quizzes.Queries;
using QuizApp.Domain.Enums;
using System.Security.Claims;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizzesController : BaseController
{
    [HttpGet]
    public async Task<ActionResult> GetQuizzes([FromQuery] GetQuizzesQuery query)
    {
        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetQuizById(Guid id)
    {
        var query = new GetQuizByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateQuiz([FromBody] CreateQuizCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Created($"/api/quizzes/{result.Value}", new { Id = result.Value });
        }
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateQuiz(Guid id, [FromBody] UpdateQuizCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteQuiz(Guid id)
    {
        var command = new DeleteQuizCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<ActionResult> ActivateQuiz(Guid id)
    {
        var command = new ActivateQuizCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult> DeactivateQuiz(Guid id)
    {
        var command = new DeactivateQuizCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("public")]
    public async Task<ActionResult> GetPublicQuizzes([FromQuery] PaginationParameters pagination)
    {
        var query = new GetQuizzesQuery
        {
            Pagination = pagination,
            IsPublic = true,
            IsActive = true
        };
        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpGet("difficulty/{difficulty}")]
    public async Task<ActionResult> GetQuizzesByDifficulty(QuizDifficulty difficulty, [FromQuery] PaginationParameters pagination)
    {
        var query = new GetQuizzesQuery
        {
            Pagination = pagination,
            Difficulty = difficulty,
            IsActive = true
        };
        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpGet("my-quizzes")]
    public async Task<ActionResult> GetMyQuizzes([FromQuery] PaginationParameters pagination)
    {
        var query = new GetQuizzesQuery
        {
            Pagination = pagination,
            CreatedByUserId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString())
        };
        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }
}