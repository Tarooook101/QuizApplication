using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.API.Controllersp;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizReviews.Commands;
using QuizApp.Application.QuizReviews.DTOs;
using QuizApp.Application.QuizReviews.Queries;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizReviewsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult> CreateQuizReview([FromBody] CreateQuizReviewDto dto)
    {
        var command = dto.Adapt<CreateQuizReviewCommand>();
        var result = await Mediator.Send(command);

        if (result.IsSuccess)
            return Created(result.Value, null);

        return HandleResult(Result.Failure(result.Error));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateQuizReview(Guid id, [FromBody] UpdateQuizReviewDto dto)
    {
        var command = dto.Adapt<UpdateQuizReviewCommand>();
        command.Id = id;
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteQuizReview(Guid id)
    {
        var command = new DeleteQuizReviewCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetQuizReview(Guid id)
    {
        var query = new GetQuizReviewByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetQuizReviews(
        [FromQuery] Guid? quizId,
        [FromQuery] Guid? userId,
        [FromQuery] bool? isPublic,
        [FromQuery] int? minRating,
        [FromQuery] int? maxRating,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetQuizReviewsQuery
        {
            QuizId = quizId,
            UserId = userId,
            IsPublic = isPublic,
            MinRating = minRating,
            MaxRating = maxRating,
            Pagination = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            }
        };

        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpGet("quiz/{quizId:guid}/summary")]
    public async Task<ActionResult> GetQuizReviewSummary(Guid quizId)
    {
        var query = new GetQuizReviewSummaryQuery { QuizId = quizId };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}