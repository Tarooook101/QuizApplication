using MediatR;
using QuizApp.Application.Common.Models;


namespace QuizApp.Application.Common.Interfaces;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}