using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Application.Common.Models;

namespace QuizApp.API.Controllersp;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (result.Value == null)
                return NotFound();

            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.Error);
    }

    protected ActionResult HandlePaginatedResult<T>(Result<PaginatedResult<T>> result)
    {
        if (result.IsSuccess)
        {
            var paginatedResult = result.Value;
            var response = new
            {
                items = paginatedResult.Items,
                totalCount = paginatedResult.TotalCount,
                pageNumber = paginatedResult.PageNumber,
                pageSize = paginatedResult.PageSize,
                totalPages = paginatedResult.TotalPages,
                hasPreviousPage = paginatedResult.HasPreviousPage,
                hasNextPage = paginatedResult.HasNextPage
            };

            return Ok(response);
        }

        return BadRequest(result.Error);
    }

    protected ActionResult Created<T>(T resource, string? location = null)
    {
        if (location != null)
            return Created(location, resource);

        return Ok(resource);
    }
}