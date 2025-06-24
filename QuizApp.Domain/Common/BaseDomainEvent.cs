namespace QuizApp.Domain.Common;

public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; protected set; }

    protected BaseDomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}