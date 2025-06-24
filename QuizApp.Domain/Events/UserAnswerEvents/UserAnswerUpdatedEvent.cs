using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.UserAnswerEvents;

public class UserAnswerUpdatedEvent : BaseDomainEvent
{
    public UserAnswer UserAnswer { get; }

    public UserAnswerUpdatedEvent(UserAnswer userAnswer)
    {
        UserAnswer = userAnswer;
    }
}