using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IQuizAttemptRepository : IRepository<QuizAttempt, int>
    {
        Task<IReadOnlyList<QuizAttempt>> GetUserAttemptsAsync(string userId, int quizId, CancellationToken cancellationToken = default);
        Task<int> GetAttemptCountAsync(string userId, int quizId, CancellationToken cancellationToken = default);
    }
}
