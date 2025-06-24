using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.API.Controllersp;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizResults.Commands;
using QuizApp.Application.QuizResults.Queries;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizResultsController : BaseController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetQuizResult(Guid id)
    {
        var result = await Mediator.Send(new GetQuizResultQuery(id));
        return HandleResult(result);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult> GetQuizResultsByUser(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetQuizResultsByUserQuery(userId)
        {
            Pagination = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            }
        };

        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpGet("quiz/{quizId:guid}")]
    public async Task<ActionResult> GetQuizResultsByQuiz(
        Guid quizId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetQuizResultsByQuizQuery(quizId)
        {
            Pagination = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            }
        };

        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpGet("quiz/{quizId:guid}/top-scores")]
    public async Task<ActionResult> GetTopScores(Guid quizId, [FromQuery] int count = 10)
    {
        var result = await Mediator.Send(new GetTopScoresQuery(quizId, count));
        return HandleResult(result);
    }

    [HttpGet("quiz/{quizId:guid}/statistics")]
    public async Task<ActionResult> GetQuizStatistics(Guid quizId)
    {
        var result = await Mediator.Send(new GetQuizStatisticsQuery(quizId));
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateQuizResult(CreateQuizResultCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/feedback")]
    public async Task<ActionResult> UpdateFeedback(Guid id, UpdateQuizResultFeedbackCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

}