using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.QuizAttemptEvents;

public class QuizAttemptAbandonedEvent : BaseDomainEvent
{
    public QuizAttempt QuizAttempt { get; }

    public QuizAttemptAbandonedEvent(QuizAttempt quizAttempt)
    {
        QuizAttempt = quizAttempt;
    }
}
