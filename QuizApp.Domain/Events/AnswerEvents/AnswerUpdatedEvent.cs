using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.AnswerEvents;

public class AnswerUpdatedEvent : BaseDomainEvent
{
    public Answer Answer { get; }

    public AnswerUpdatedEvent(Answer answer)
    {
        Answer = answer;
    }
}