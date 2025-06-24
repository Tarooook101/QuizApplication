using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;


namespace QuizApp.Domain.Events.QuizReviewEvents;

public class QuizReviewUpdatedEvent : BaseDomainEvent
{
    public QuizReview QuizReview { get; }

    public QuizReviewUpdatedEvent(QuizReview quizReview)
    {
        QuizReview = quizReview;
    }
}
