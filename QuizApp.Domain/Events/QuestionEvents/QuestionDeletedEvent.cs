using QuizApp.Domain.Common;


namespace QuizApp.Domain.Events.QuestionEvents;

public class QuestionDeletedEvent : BaseDomainEvent
{
    public Guid QuestionId { get; }
    public Guid QuizId { get; }

    public QuestionDeletedEvent(Guid questionId, Guid quizId)
    {
        QuestionId = questionId;
        QuizId = quizId;
    }
}