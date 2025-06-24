using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.QuestionEvents;

public class QuestionUpdatedEvent : BaseDomainEvent
{
    public Question Question { get; }

    public QuestionUpdatedEvent(Question question)
    {
        Question = question;
    }
}