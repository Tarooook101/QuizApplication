using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IQuestionResponseRepository : IRepository<QuestionResponse, int>
    {
        Task<IReadOnlyList<QuestionResponse>> GetByAttemptIdAsync(int attemptId, CancellationToken cancellationToken = default);
        Task<bool> HasUserAnsweredQuestionAsync(string userId, int questionId, CancellationToken cancellationToken = default);
    }
}
