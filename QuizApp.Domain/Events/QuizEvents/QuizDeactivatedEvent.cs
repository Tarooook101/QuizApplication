using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Events.QuizEvents;

public class QuizDeactivatedEvent : BaseDomainEvent
{
    public Quiz Quiz { get; }

    public QuizDeactivatedEvent(Quiz quiz)
    {
        Quiz = quiz;
    }
}