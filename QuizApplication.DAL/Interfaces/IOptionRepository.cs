using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IOptionRepository : IRepository<Option, int>
    {
        Task<IReadOnlyList<Option>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default);
        Task UpdateOptionOrdersAsync(IEnumerable<(int OptionId, int NewOrder)> optionOrders, CancellationToken cancellationToken = default);
    }
}
