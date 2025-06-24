using FluentValidation;
using MediatR;
using QuizApp.Application.Common.Models;

namespace QuizApp.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var resultType = typeof(TResponse).GetGenericArguments()[0];
                    var failureMethod = typeof(Result).GetMethod(nameof(Result.Failure))?.MakeGenericMethod(resultType);
                    return (TResponse)failureMethod?.Invoke(null, new object[] { errorMessage })!;
                }

                if (typeof(TResponse) == typeof(Result))
                {
                    return (TResponse)(object)Result.Failure(errorMessage);
                }

                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}