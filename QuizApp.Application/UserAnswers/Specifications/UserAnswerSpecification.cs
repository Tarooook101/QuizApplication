using QuizApp.Domain.Entities;
using QuizApp.Domain.Specifications;
using System.Linq.Expressions;


namespace QuizApp.Application.UserAnswers.Specifications;

public class UserAnswerSpecification : BaseSpecification<UserAnswer>
{
    public UserAnswerSpecification(
        Guid? quizAttemptId = null,
        Guid? questionId = null,
        bool? isCorrect = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        Expression<Func<UserAnswer, bool>> criteria = x => true;

        if (quizAttemptId.HasValue)
        {
            var prev = criteria;
            criteria = x => prev.Compile().Invoke(x) && x.QuizAttemptId == quizAttemptId.Value;
        }

        if (questionId.HasValue)
        {
            var prev = criteria;
            criteria = x => prev.Compile().Invoke(x) && x.QuestionId == questionId.Value;
        }

        if (isCorrect.HasValue)
        {
            var prev = criteria;
            criteria = x => prev.Compile().Invoke(x) && x.IsCorrect == isCorrect.Value;
        }

        Criteria = criteria;

        AddInclude(x => x.Question);
        AddInclude(x => x.SelectedAnswer!);
        AddInclude(x => x.QuizAttempt);

        ApplyOrderByDescending(x => x.AnsweredAt);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}
