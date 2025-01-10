using QuizApplication.BLL.DTOs;
using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Interfaces
{
    public interface IQuestionService : IService<Question, int>
    {
        Task<IReadOnlyList<Question>> GetQuestionsByQuizAsync(int quizId, CancellationToken cancellationToken = default);
        Task<Question> AddQuestionToQuizAsync(int quizId, Question question, CancellationToken cancellationToken = default);
        Task UpdateQuestionOrderAsync(int quizId, Dictionary<int, int> questionOrders, CancellationToken cancellationToken = default);
        Task<QuestionStatistics> GetQuestionStatisticsAsync(int questionId, CancellationToken cancellationToken = default);
    }
}
