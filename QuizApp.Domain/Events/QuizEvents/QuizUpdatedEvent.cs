using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Events.QuizEvents;

public class QuizUpdatedEvent : BaseDomainEvent
{
    public Quiz Quiz { get; }

    public QuizUpdatedEvent(Quiz quiz)
    {
        Quiz = quiz;
    }
}
