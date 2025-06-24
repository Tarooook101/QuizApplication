using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Events.UserAnswerEvents;

public class UserAnswerCreatedEvent : BaseDomainEvent
{
    public UserAnswer UserAnswer { get; }

    public UserAnswerCreatedEvent(UserAnswer userAnswer)
    {
        UserAnswer = userAnswer;
    }
}