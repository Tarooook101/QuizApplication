using QuizApp.Domain.Common;


namespace QuizApp.Domain.Events.AnswerEvents;

public class AnswerDeletedEvent : BaseDomainEvent
{
    public Guid AnswerId { get; }
    public Guid QuestionId { get; }

    public AnswerDeletedEvent(Guid answerId, Guid questionId)
    {
        AnswerId = answerId;
        QuestionId = questionId;
    }
}