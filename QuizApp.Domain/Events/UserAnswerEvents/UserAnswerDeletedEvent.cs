using QuizApp.Domain.Common;


namespace QuizApp.Domain.Events.UserAnswerEvents;

public class UserAnswerDeletedEvent : BaseDomainEvent
{
    public Guid UserAnswerId { get; }
    public Guid QuizAttemptId { get; }
    public Guid QuestionId { get; }

    public UserAnswerDeletedEvent(Guid userAnswerId, Guid quizAttemptId, Guid questionId)
    {
        UserAnswerId = userAnswerId;
        QuizAttemptId = quizAttemptId;
        QuestionId = questionId;
    }
}