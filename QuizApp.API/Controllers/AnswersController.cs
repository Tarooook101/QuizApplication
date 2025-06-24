using Mapster;
using Microsoft.AspNetCore.Mvc;
using QuizApp.API.Controllersp;
using QuizApp.Application.Answers.Commands;
using QuizApp.Application.Answers.DTOs;
using QuizApp.Application.Answers.Queries;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnswersController : BaseController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetAnswerByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("question/{questionId:guid}")]
    public async Task<ActionResult> GetByQuestionId(Guid questionId, CancellationToken cancellationToken)
    {
        var query = new GetAnswersByQuestionIdQuery(questionId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateAnswerDto createAnswerDto, CancellationToken cancellationToken)
    {
        var command = createAnswerDto.Adapt<CreateAnswerCommand>();
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return Created($"/api/answers/{result.Value.Id}", result.Value);

        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateAnswerDto updateAnswerDto, CancellationToken cancellationToken)
    {
        var command = updateAnswerDto.Adapt<UpdateAnswerCommand>();
        command.Id = id;

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteAnswerCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}