using Microsoft.Extensions.Logging;
using QuizApplication.BLL.Interfaces;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AchievementService> _logger;
        private readonly ICacheService _cacheService;

        private const string CACHE_KEY_PREFIX = "achievement_";
        private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromHours(1);

        public AchievementService(
            IUnitOfWork unitOfWork,
            ILogger<AchievementService> logger,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Achievement?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}{id}";
                var cachedAchievement = await _cacheService.GetAsync<Achievement>(cacheKey, cancellationToken);
                if (cachedAchievement != null)
                    return cachedAchievement;

                var achievement = await _unitOfWork.Achievements.GetByIdAsync(id, cancellationToken);
                if (achievement != null)
                    await _cacheService.SetAsync(cacheKey, achievement, CACHE_DURATION, cancellationToken);

                return achievement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting achievement with ID: {AchievementId}", id);
                throw new ServiceException("Failed to retrieve achievement", ex);
            }
        }

        public async Task<IReadOnlyList<Achievement>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _unitOfWork.Achievements.GetAllAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all achievements");
                throw new ServiceException("Failed to retrieve achievements", ex);
            }
        }

        public async Task<Achievement> CreateAsync(Achievement achievement, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateAchievement(achievement);

                await _unitOfWork.Achievements.AddAsync(achievement, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return achievement;
            }
            catch (Exception ex) when (ex is not ValidationException)
            {
                _logger.LogError(ex, "Error occurred while creating achievement: {AchievementName}", achievement.Name);
                throw new ServiceException("Failed to create achievement", ex);
            }
        }

        public async Task UpdateAsync(Achievement achievement, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateAchievement(achievement);

                var existingAchievement = await _unitOfWork.Achievements.GetByIdAsync(achievement.Id, cancellationToken)
                    ?? throw new NotFoundException("Achievement", achievement.Id);

                var cacheKey = $"{CACHE_KEY_PREFIX}{achievement.Id}";
                await _cacheService.RemoveAsync(cacheKey, cancellationToken);

                await _unitOfWork.Achievements.UpdateAsync(achievement, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error occurred while updating achievement: {AchievementId}", achievement.Id);
                throw new ServiceException("Failed to update achievement", ex);
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var achievement = await _unitOfWork.Achievements.GetByIdAsync(id, cancellationToken)
                    ?? throw new NotFoundException("Achievement", id);

                // Check if any users have earned this achievement
                var hasEarned = await _unitOfWork.UserAchievements
                    .AnyAsync(ua => ua.AchievementId == id, cancellationToken);

                if (hasEarned)
                    throw new ValidationException("Cannot delete achievement that has been earned by users");

                var cacheKey = $"{CACHE_KEY_PREFIX}{id}";
                await _cacheService.RemoveAsync(cacheKey, cancellationToken);

                await _unitOfWork.Achievements.DeleteAsync(achievement, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
            {
                _logger.LogError(ex, "Error occurred while deleting achievement: {AchievementId}", id);
                throw new ServiceException("Failed to delete achievement", ex);
            }
        }

        public async Task<IReadOnlyList<Achievement>> GetAvailableAchievementsAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Get all achievements
                var allAchievements = await _unitOfWork.Achievements.GetAvailableAchievementsAsync(userId, cancellationToken);

                // Filter out achievements the user already has
                var userAchievements = await _unitOfWork.UserAchievements
                    .GetByUserIdAsync(userId, cancellationToken);

                var earnedAchievementIds = userAchievements.Select(ua => ua.AchievementId).ToHashSet();

                return allAchievements
                    .Where(a => !earnedAchievementIds.Contains(a.Id))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting available achievements for user: {UserId}", userId);
                throw new ServiceException("Failed to retrieve available achievements", ex);
            }
        }

        public async Task<UserAchievement> AwardAchievementAsync(string userId, int achievementId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if achievement exists
                var achievement = await _unitOfWork.Achievements.GetByIdAsync(achievementId, cancellationToken)
                    ?? throw new NotFoundException("Achievement", achievementId);

                // Check if user already has this achievement
                var hasAchievement = await _unitOfWork.UserAchievements
                    .HasAchievementAsync(userId, achievementId, cancellationToken);

                if (hasAchievement)
                    throw new ValidationException("User already has this achievement");

                // Create new user achievement
                var userAchievement = new UserAchievement
                {
                    UserId = userId,
                    AchievementId = achievementId,
                    AwardedDate = DateTimeOffset.UtcNow
                };

                await _unitOfWork.UserAchievements.AddAsync(userAchievement, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return userAchievement;
            }
            catch (Exception ex) when (ex is not ValidationException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error occurred while awarding achievement {AchievementId} to user {UserId}",
                    achievementId, userId);
                throw new ServiceException("Failed to award achievement", ex);
            }
        }

        public async Task<bool> CheckUserEligibilityAsync(string userId, int achievementId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _unitOfWork.Achievements.CheckEligibilityAsync(userId, achievementId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking eligibility for achievement {AchievementId} for user {UserId}",
                    achievementId, userId);
                throw new ServiceException("Failed to check achievement eligibility", ex);
            }
        }

        public async Task ProcessQuizCompletionAchievementsAsync(string userId, int quizId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Get quiz attempt information
                var attempts = await _unitOfWork.QuizAttempts
                    .GetUserAttemptsAsync(userId, quizId, cancellationToken);

                var latestAttempt = attempts.OrderByDescending(a => a.CompletedAt).FirstOrDefault();
                if (latestAttempt == null || !latestAttempt.IsCompleted)
                    return;

                // Process different achievement types
                var achievementsToAward = new List<Achievement>();

                // First Attempt Achievement
                if (attempts.Count == 1)
                {
                    var firstAttemptAchievement = await _unitOfWork.Achievements
                        .FirstOrDefaultAsync(a => a.Type == AchievementType.FirstAttempt, cancellationToken);
                    if (firstAttemptAchievement != null)
                        achievementsToAward.Add(firstAttemptAchievement);
                }

                // Perfect Score Achievement
                if (latestAttempt.IsPassed && latestAttempt.Score == 100)
                {
                    var perfectScoreAchievement = await _unitOfWork.Achievements
                        .FirstOrDefaultAsync(a => a.Type == AchievementType.PerfectScore, cancellationToken);
                    if (perfectScoreAchievement != null)
                        achievementsToAward.Add(perfectScoreAchievement);
                }

                // Award achievements
                foreach (var achievement in achievementsToAward)
                {
                    var hasAchievement = await _unitOfWork.UserAchievements
                        .HasAchievementAsync(userId, achievement.Id, cancellationToken);

                    if (!hasAchievement)
                    {
                        await AwardAchievementAsync(userId, achievement.Id, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing quiz completion achievements for user {UserId} and quiz {QuizId}",
                    userId, quizId);
                throw new ServiceException("Failed to process quiz completion achievements", ex);
            }
        }

        private static void ValidateAchievement(Achievement achievement)
        {
            if (string.IsNullOrWhiteSpace(achievement.Name))
                throw new ValidationException("Achievement name is required");

            if (string.IsNullOrWhiteSpace(achievement.Description))
                throw new ValidationException("Achievement description is required");

            if (string.IsNullOrWhiteSpace(achievement.IconUrl))
                throw new ValidationException("Achievement icon URL is required");

            if (achievement.RequiredPoints < 0)
                throw new ValidationException("Required points cannot be negative");

            if (!achievement.Criteria.Any())
                throw new ValidationException("Achievement must have at least one criterion");
        }
    }
}
