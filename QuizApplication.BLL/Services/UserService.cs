using QuizApplication.BLL.DTOs;
using QuizApplication.BLL.Interfaces;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private const string CACHE_KEY_PREFIX = "user_";
        private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromMinutes(15);

        public UserService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var cacheKey = $"{CACHE_KEY_PREFIX}{userId}";
            var cachedUser = await _cacheService.GetAsync<ApplicationUser>(cacheKey, cancellationToken);

            if (cachedUser != null)
                return cachedUser;

            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException(nameof(ApplicationUser), userId);

            await _cacheService.SetAsync(cacheKey, user, CACHE_DURATION, cancellationToken);
            return user;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            var cacheKey = $"{CACHE_KEY_PREFIX}email_{email.ToLower()}";
            var cachedUser = await _cacheService.GetAsync<ApplicationUser>(cacheKey, cancellationToken);

            if (cachedUser != null)
                return cachedUser;

            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken)
                ?? throw new NotFoundException($"User with email '{email}' not found.");

            await _cacheService.SetAsync(cacheKey, user, CACHE_DURATION, cancellationToken);
            return user;
        }

        public async Task<UserProfile> GetUserProfileAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var cacheKey = $"{CACHE_KEY_PREFIX}{userId}_profile";
            var cachedProfile = await _cacheService.GetAsync<UserProfile>(cacheKey, cancellationToken);

            if (cachedProfile != null)
                return cachedProfile;

            var profile = await _unitOfWork.UserProfiles.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException(nameof(UserProfile), userId);

            await _cacheService.SetAsync(cacheKey, profile, CACHE_DURATION, cancellationToken);
            return profile;
        }

        public async Task UpdateUserProfileAsync(string userId, UserProfile profile, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));
            if (userId != profile.UserId)
                throw new ValidationException("User ID mismatch between parameters and profile.");

            // Validate profile data
            ValidateUserProfile(profile);

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                await _unitOfWork.UserProfiles.UpdateAsync(profile, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // Invalidate cache
                await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{userId}_profile", cancellationToken);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task<IReadOnlyList<UserAchievement>> GetUserAchievementsAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var cacheKey = $"{CACHE_KEY_PREFIX}{userId}_achievements";
            var cachedAchievements = await _cacheService.GetAsync<IReadOnlyList<UserAchievement>>(cacheKey, cancellationToken);

            if (cachedAchievements != null)
                return cachedAchievements;

            var achievements = await _unitOfWork.UserAchievements.GetByUserIdAsync(userId, cancellationToken);

            await _cacheService.SetAsync(cacheKey, achievements, CACHE_DURATION, cancellationToken);
            return achievements;
        }

        public async Task<UserStatistics> GetUserStatisticsAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var cacheKey = $"{CACHE_KEY_PREFIX}{userId}_statistics";
            var cachedStats = await _cacheService.GetAsync<UserStatistics>(cacheKey, cancellationToken);

            if (cachedStats != null)
                return cachedStats;

            // Get all quiz attempts for the user - fixed the null parameter issue
            var attempts = await _unitOfWork.QuizAttempts.FindAsync(
                qa => qa.UserId == userId,
                cancellationToken);

            var completedAttempts = attempts
                .Where(a => a.IsCompleted)
                .ToList();

            var statistics = new UserStatistics
            {
                TotalQuizAttempts = attempts.Count(),  // Fixed Count to Count()
                CompletedQuizzes = completedAttempts.Count(),  // Fixed Count to Count()
                AverageScore = completedAttempts.Any()
                    ? completedAttempts.Average(a => a.Score)
                    : 0,
                TotalAchievements = await _unitOfWork.UserAchievements.CountAsync(
                    ua => ua.UserId == userId,
                    cancellationToken),
                AverageQuizCompletionTime = completedAttempts.Any()
                    ? TimeSpan.FromTicks((long)completedAttempts.Average(a => a.Duration?.Ticks ?? 0))
                    : TimeSpan.Zero,
                CompletedQuizzesByDifficulty = completedAttempts
                    .GroupBy(a => a.Quiz.Difficulty)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            await _cacheService.SetAsync(cacheKey, statistics, CACHE_DURATION, cancellationToken);
            return statistics;
        }

        private void ValidateUserProfile(UserProfile profile)
        {
            if (string.IsNullOrWhiteSpace(profile.TimeZone))
                throw new ValidationException("TimeZone is required.");

            if (string.IsNullOrWhiteSpace(profile.Language))
                throw new ValidationException("Language is required.");

            if (profile.Biography?.Length > 1000)
                throw new ValidationException("Biography cannot exceed 1000 characters.");

            if (profile.NotificationPreferences == null)
                throw new ValidationException("NotificationPreferences cannot be null.");
        }
    }
}
