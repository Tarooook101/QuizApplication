using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.QuizAttemptEvents;

public class QuizAttemptCompletedEvent : BaseDomainEvent
{
    public QuizAttempt QuizAttempt { get; }

    public QuizAttemptCompletedEvent(QuizAttempt quizAttempt)
    {
        QuizAttempt = quizAttempt;
    }
}