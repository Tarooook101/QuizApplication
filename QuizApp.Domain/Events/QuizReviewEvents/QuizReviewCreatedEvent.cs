using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Events.QuizReviewEvents;

public class QuizReviewCreatedEvent : BaseDomainEvent
{
    public QuizReview QuizReview { get; }

    public QuizReviewCreatedEvent(QuizReview quizReview)
    {
        QuizReview = quizReview;
    }
}
