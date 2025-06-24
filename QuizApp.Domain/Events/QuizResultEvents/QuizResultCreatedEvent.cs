using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.QuizResultEvents;

public class QuizResultCreatedEvent : BaseDomainEvent
{
    public QuizResult QuizResult { get; }

    public QuizResultCreatedEvent(QuizResult quizResult)
    {
        QuizResult = quizResult;
    }
}