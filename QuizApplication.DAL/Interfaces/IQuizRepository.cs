using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IQuizRepository : IRepository<Quiz, int>
    {
        Task<IReadOnlyList<Quiz>> GetActiveQuizzesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Quiz>> GetQuizzesByUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> IsUserEligibleForQuizAsync(string userId, int quizId, CancellationToken cancellationToken = default);
    }
}
