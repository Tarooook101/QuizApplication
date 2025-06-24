using QuizApp.Domain.Entities;
using QuizApp.Domain.Specifications;


namespace QuizApp.Application.QuizAttempts.Specifications;

public class QuizAttemptsByUserSpecification : BaseSpecification<QuizAttempt>
{
    public QuizAttemptsByUserSpecification(Guid userId, int skip, int take) : base(qa => qa.UserId == userId)
    {
        ApplyOrderByDescending(qa => qa.StartedAt);
        ApplyPaging(skip, take);
    }
}