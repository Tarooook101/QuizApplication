using QuizApp.Domain.Entities;
using QuizApp.Domain.Specifications;


namespace QuizApp.Application.UserAnswers.Specifications;

public class UserAnswerDetailsSpecification : BaseSpecification<UserAnswer>
{
    public UserAnswerDetailsSpecification(Guid userAnswerId) : base(x => x.Id == userAnswerId)
    {
        AddInclude(x => x.Question);
        AddInclude(x => x.SelectedAnswer);
        AddInclude(x => x.QuizAttempt);
    }
}