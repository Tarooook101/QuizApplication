using QuizApplication.BLL.DTOs;
using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Interfaces
{
    public interface IQuizService : IService<Quiz, int>
    {
        Task<IReadOnlyList<Quiz>> GetQuizzesByUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Quiz>> GetActiveQuizzesAsync(CancellationToken cancellationToken = default);
        Task<bool> IsUserEligibleForQuizAsync(string userId, int quizId, CancellationToken cancellationToken = default);
        Task<Quiz> PublishQuizAsync(int quizId, CancellationToken cancellationToken = default);
        Task<Quiz> UnpublishQuizAsync(int quizId, CancellationToken cancellationToken = default);
        Task<QuizStatistics> GetQuizStatisticsAsync(int quizId, CancellationToken cancellationToken = default);
    }
}
