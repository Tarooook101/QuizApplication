using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Events.QuizReviewEvents;

public class QuizReviewMadePublicEvent : BaseDomainEvent
{
    public QuizReview QuizReview { get; }

    public QuizReviewMadePublicEvent(QuizReview quizReview)
    {
        QuizReview = quizReview;
    }
}