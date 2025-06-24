using QuizApp.Domain.Entities;
using QuizApp.Domain.Specifications;


namespace QuizApp.Application.QuizAttempts.Specifications;

public class QuizAttemptsByQuizSpecification : BaseSpecification<QuizAttempt>
{
    public QuizAttemptsByQuizSpecification(Guid quizId, int skip, int take) : base(qa => qa.QuizId == quizId)
    {
        ApplyOrderByDescending(qa => qa.StartedAt);
        ApplyPaging(skip, take);
    }
}