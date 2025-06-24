using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.API.Controllersp;
using QuizApp.Application.Questions.Commands;
using QuizApp.Application.Questions.Queries;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : BaseController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var query = new GetQuestionByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("quiz/{quizId:guid}")]
    public async Task<ActionResult> GetByQuizId(Guid quizId, [FromQuery] bool orderByIndex = true)
    {
        var query = new GetQuestionsByQuizIdQuery(quizId, orderByIndex);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] GetQuestionsQuery query)
    {
        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateQuestionCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Created($"api/questions/{result.Value}", new { id = result.Value });
        }
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateQuestionCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Question ID mismatch");
        }

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var command = new DeleteQuestionCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("reorder")]
    public async Task<ActionResult> Reorder([FromBody] ReorderQuestionsCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}