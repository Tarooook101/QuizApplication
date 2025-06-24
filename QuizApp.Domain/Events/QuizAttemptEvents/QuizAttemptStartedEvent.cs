using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.QuizAttemptEvents;

public class QuizAttemptStartedEvent : BaseDomainEvent
{
    public QuizAttempt QuizAttempt { get; }

    public QuizAttemptStartedEvent(QuizAttempt quizAttempt)
    {
        QuizAttempt = quizAttempt;
    }
}