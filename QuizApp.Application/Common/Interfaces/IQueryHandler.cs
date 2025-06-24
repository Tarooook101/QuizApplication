using MediatR;
using QuizApp.Application.Common.Models;

namespace QuizApp.Application.Common.Interfaces;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}