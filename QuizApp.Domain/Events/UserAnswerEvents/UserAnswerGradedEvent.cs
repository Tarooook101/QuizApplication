using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.UserAnswerEvents;

public class UserAnswerGradedEvent : BaseDomainEvent
{
    public UserAnswer UserAnswer { get; }

    public UserAnswerGradedEvent(UserAnswer userAnswer)
    {
        UserAnswer = userAnswer;
    }
}