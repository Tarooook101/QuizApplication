using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.QuizEvents;

public class QuizCreatedEvent : BaseDomainEvent
{
    public Quiz Quiz { get; }

    public QuizCreatedEvent(Quiz quiz)
    {
        Quiz = quiz;
    }
}