using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IQuestionRepository : IRepository<Question, int>
    {
        Task<IReadOnlyList<Question>> GetByQuizIdAsync(int quizId, bool includeOptions = true, CancellationToken cancellationToken = default);
        Task UpdateQuestionOrdersAsync(IEnumerable<(int QuestionId, int NewOrder)> questionOrders, CancellationToken cancellationToken = default);
    }
}
