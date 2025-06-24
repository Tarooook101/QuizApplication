using QuizApp.Domain.Entities;
using QuizApp.Domain.Specifications;


namespace QuizApp.Application.QuizAttempts.Specifications;

public class QuizAttemptsPaginatedSpecification : BaseSpecification<QuizAttempt>
{
    public QuizAttemptsPaginatedSpecification(int skip, int take) : base()
    {
        ApplyOrderByDescending(qa => qa.StartedAt);
        ApplyPaging(skip, take);
    }
}