using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.AnswerEvents;

public class AnswerCreatedEvent : BaseDomainEvent
{
    public Answer Answer { get; }

    public AnswerCreatedEvent(Answer answer)
    {
        Answer = answer;
    }
}