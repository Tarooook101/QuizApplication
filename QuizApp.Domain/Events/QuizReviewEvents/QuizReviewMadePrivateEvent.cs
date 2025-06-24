using QuizApp.Domain.Common;
using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Events.QuizReviewEvents;

public class QuizReviewMadePrivateEvent : BaseDomainEvent
{
    public QuizReview QuizReview { get; }

    public QuizReviewMadePrivateEvent(QuizReview quizReview)
    {
        QuizReview = quizReview;
    }
}