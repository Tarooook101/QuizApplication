using QuizApplication.BLL.DTOs;
using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Interfaces
{
    public interface IQuizAttemptService : IService<QuizAttempt, int>
    {
        Task<QuizAttempt> StartQuizAttemptAsync(string userId, int quizId, CancellationToken cancellationToken = default);
        Task<QuizAttempt> SubmitQuizAttemptAsync(int attemptId, IEnumerable<QuestionResponse> responses, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuizAttempt>> GetUserAttemptsAsync(string userId, int quizId, CancellationToken cancellationToken = default);
        Task<QuizAttemptResult> GetAttemptResultAsync(int attemptId, CancellationToken cancellationToken = default);
        Task<bool> CanUserAttemptQuizAsync(string userId, int quizId, CancellationToken cancellationToken = default);
    }
}
