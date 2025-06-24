using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.QuizEvents;

public class QuizActivatedEvent : BaseDomainEvent
{
    public Quiz Quiz { get; }

    public QuizActivatedEvent(Quiz quiz)
    {
        Quiz = quiz;
    }
}
