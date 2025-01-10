using QuizApplication.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Interfaces
{
    public interface IUserRepository : IRepository<ApplicationUser, string>
    {
        Task<ApplicationUser> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
