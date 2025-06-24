using MediatR;
using QuizApp.Application.Common.Models;

namespace QuizApp.Application.Common.Interfaces;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}