using Microsoft.EntityFrameworkCore;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Database;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Repositories
{
    public class UserRepository : Repository<ApplicationUser, string>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<ApplicationUser> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken)
                ?? throw new InvalidOperationException($"User with email {email} not found.");
        }

        public override async Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.Profile)
                .Include(u => u.Achievements)
                    .ThenInclude(ua => ua.Achievement)
                .Include(u => u.QuizAttempts.Where(qa => qa.Status == QuizAttemptStatus.Completed))
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
    }
}
