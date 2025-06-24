using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.API.Controllersp;
using QuizApp.Application.UserAnswers.Commands;
using QuizApp.Application.UserAnswers.DTOs;
using QuizApp.Application.UserAnswers.Queries;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserAnswersController : BaseController
{
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUserAnswer(Guid id)
    {
        var query = new GetUserAnswerByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("quiz-attempt/{quizAttemptId}")]
    public async Task<ActionResult> GetUserAnswersByQuizAttempt(Guid quizAttemptId)
    {
        var query = new GetUserAnswersByQuizAttemptQuery { QuizAttemptId = quizAttemptId };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("question/{questionId}")]
    public async Task<ActionResult> GetUserAnswersByQuestion(Guid questionId)
    {
        var query = new GetUserAnswersByQuestionQuery { QuestionId = questionId };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetUserAnswers([FromQuery] GetUserAnswersQuery query)
    {
        var result = await Mediator.Send(query);
        return HandlePaginatedResult(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUserAnswer([FromBody] CreateUserAnswerDto dto)
    {
        var command = dto.Adapt<CreateUserAnswerCommand>();
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUserAnswer(Guid id, [FromBody] UpdateUserAnswerDto dto)
    {
        var command = dto.Adapt<UpdateUserAnswerCommand>();
        command.Id = id;
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUserAnswer(Guid id)
    {
        var command = new DeleteUserAnswerCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("{id}/grade")]
    public async Task<ActionResult> GradeUserAnswer(Guid id, [FromBody] GradeUserAnswerCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
