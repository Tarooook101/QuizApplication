using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IQuizTagRepository : IRepository<QuizTag, int>
    {
        Task<IReadOnlyList<QuizTag>> GetByQuizIdAsync(int quizId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Quiz>> GetQuizzesByTagAsync(int tagId, CancellationToken cancellationToken = default);
    }
}
