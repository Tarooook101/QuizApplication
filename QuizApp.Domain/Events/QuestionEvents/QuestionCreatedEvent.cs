using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.QuestionEvents;

public class QuestionCreatedEvent : BaseDomainEvent
{
    public Question Question { get; }

    public QuestionCreatedEvent(Question question)
    {
        Question = question;
    }
}
