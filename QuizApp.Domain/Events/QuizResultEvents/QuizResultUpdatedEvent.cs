using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.QuizResultEvents;

public class QuizResultUpdatedEvent : BaseDomainEvent
{
    public QuizResult QuizResult { get; }

    public QuizResultUpdatedEvent(QuizResult quizResult)
    {
        QuizResult = quizResult;
    }
}