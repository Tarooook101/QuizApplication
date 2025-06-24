using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.API.Controllersp;
using QuizApp.Application.Categories.Commands;
using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Categories.Queries;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : BaseController
{
    [HttpGet]
    public async Task<ActionResult> GetCategories([FromQuery] bool? isActive = null)
    {
        var query = new GetAllCategoriesQuery(isActive);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("paginated")]
    public async Task<ActionResult> GetCategoriesPaginated([FromQuery] GetCategoriesPaginatedQuery query)
    {
        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetCategory(Guid id)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        var command = dto.Adapt<CreateCategoryCommand>();
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto dto)
    {
        var command = dto.Adapt<UpdateCategoryCommand>() with { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/toggle-status")]
    public async Task<ActionResult> ToggleCategoryStatus(Guid id)
    {
        var command = new ToggleCategoryStatusCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}