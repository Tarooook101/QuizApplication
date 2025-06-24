using MediatR;


namespace QuizApp.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}