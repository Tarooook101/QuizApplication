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
    public class AchievementRepository : Repository<Achievement, int>, IAchievementRepository
    {
        private readonly ApplicationDbContext _context;

        public AchievementRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Achievement>> GetAvailableAchievementsAsync(string userId, CancellationToken cancellationToken = default)
        {
            var userAchievements = await _context.UserAchievements
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.AchievementId)
                .ToListAsync(cancellationToken);

            return await _dbSet
                .Where(a => !userAchievements.Contains(a.Id))
                .OrderBy(a => a.RequiredPoints)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> CheckEligibilityAsync(string userId, int achievementId, CancellationToken cancellationToken = default)
        {
            var achievement = await _dbSet.FindAsync(new object[] { achievementId }, cancellationToken);
            if (achievement == null)
                return false;

            switch (achievement.Type)
            {
                case AchievementType.QuizCompletion:
                    return await CheckQuizCompletionEligibilityAsync(userId, achievement, cancellationToken);
                case AchievementType.HighScore:
                    return await CheckHighScoreEligibilityAsync(userId, achievement, cancellationToken);
                case AchievementType.Streak:
                    return await CheckStreakEligibilityAsync(userId, achievement, cancellationToken);
                default:
                    return false;
            }
        }

        private async Task<bool> CheckQuizCompletionEligibilityAsync(
            string userId,
            Achievement achievement,
            CancellationToken cancellationToken)
        {
            var completedQuizCount = await _context.QuizAttempts
                .CountAsync(qa => qa.UserId == userId &&
                                 qa.Status == QuizAttemptStatus.Completed,
                           cancellationToken);

            return completedQuizCount >= achievement.RequiredPoints;
        }

        private async Task<bool> CheckHighScoreEligibilityAsync(
            string userId,
            Achievement achievement,
            CancellationToken cancellationToken)
        {
            var highestScore = await _context.QuizAttempts
                .Where(qa => qa.UserId == userId &&
                            qa.Status == QuizAttemptStatus.Completed)
                .MaxAsync(qa => (int?)qa.Score, cancellationToken);

            return highestScore.GetValueOrDefault() >= achievement.RequiredPoints;
        }

        private async Task<bool> CheckStreakEligibilityAsync(
            string userId,
            Achievement achievement,
            CancellationToken cancellationToken)
        {
            var attempts = await _context.QuizAttempts
                .Where(qa => qa.UserId == userId &&
                            qa.Status == QuizAttemptStatus.Completed)
                .OrderBy(qa => qa.CompletedAt)
                .ToListAsync(cancellationToken);

            int currentStreak = 0;
            DateTimeOffset? lastAttemptDate = null;

            foreach (var attempt in attempts)
            {
                if (!lastAttemptDate.HasValue ||
                    attempt.CompletedAt == lastAttemptDate.Value.Date.AddDays(1))
                {
                    currentStreak++;
                }
                else if (attempt.CompletedAt > lastAttemptDate.Value.Date.AddDays(1))
                {
                    currentStreak = 1;
                }
                lastAttemptDate = attempt.CompletedAt;
            }

            return currentStreak >= achievement.RequiredPoints;
        }
    }
}
