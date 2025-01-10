using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuizApplication.BLL.DTOs;
using QuizApplication.BLL.Interfaces;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Services
{
    public class QuizService : IQuizService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuizService> _logger;
        private readonly ICacheService _cacheService;

        public QuizService(
            IUnitOfWork unitOfWork,
            ILogger<QuizService> logger,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Quiz?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = $"quiz_{id}";
                var cachedQuiz = await _cacheService.GetAsync<Quiz>(cacheKey);

                if (cachedQuiz != null)
                    return cachedQuiz;

                var quiz = await _unitOfWork.Quizzes.GetByIdAsync(id, cancellationToken);

                if (quiz != null)
                    await _cacheService.SetAsync(cacheKey, quiz, TimeSpan.FromMinutes(30));

                return quiz;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quiz with ID {QuizId}", id);
                throw new ServiceException("Error retrieving quiz", ex);
            }
        }

        public async Task<IReadOnlyList<Quiz>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _unitOfWork.Quizzes.GetAllAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all quizzes");
                throw new ServiceException("Error retrieving quizzes", ex);
            }
        }

        public async Task<Quiz> CreateAsync(Quiz quiz, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateQuiz(quiz);

                quiz.Status = QuizStatus.Draft;
                quiz.CreatedAt = DateTimeOffset.UtcNow;

                var createdQuiz = await _unitOfWork.Quizzes.AddAsync(quiz, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created new quiz with ID {QuizId}", createdQuiz.Id);
                return createdQuiz;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz");
                throw new ServiceException("Error creating quiz", ex);
            }
        }

        public async Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateQuiz(quiz);

                var existingQuiz = await _unitOfWork.Quizzes.GetByIdAsync(quiz.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Quiz), quiz.Id);

                if (existingQuiz.Status == QuizStatus.Published && quiz.Status == QuizStatus.Draft)
                    throw new ValidationException("Cannot change published quiz back to draft status");

                quiz.LastModifiedAt = DateTimeOffset.UtcNow;
                await _unitOfWork.Quizzes.UpdateAsync(quiz, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheService.RemoveAsync($"quiz_{quiz.Id}");
                _logger.LogInformation("Updated quiz with ID {QuizId}", quiz.Id);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quiz with ID {QuizId}", quiz.Id);
                throw new ServiceException("Error updating quiz", ex);
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var quiz = await _unitOfWork.Quizzes.GetByIdAsync(id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Quiz), id);

                if (quiz.Status == QuizStatus.Published)
                    throw new ValidationException("Cannot delete a published quiz");

                await _unitOfWork.Quizzes.DeleteAsync(quiz, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheService.RemoveAsync($"quiz_{id}");
                _logger.LogInformation("Deleted quiz with ID {QuizId}", id);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quiz with ID {QuizId}", id);
                throw new ServiceException("Error deleting quiz", ex);
            }
        }

        public async Task<IReadOnlyList<Quiz>> GetQuizzesByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _unitOfWork.Quizzes.GetQuizzesByUserAsync(userId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes for user {UserId}", userId);
                throw new ServiceException("Error retrieving user quizzes", ex);
            }
        }

        public async Task<IReadOnlyList<Quiz>> GetActiveQuizzesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = "active_quizzes";
                var cachedQuizzes = await _cacheService.GetAsync<IReadOnlyList<Quiz>>(cacheKey);

                if (cachedQuizzes != null)
                    return cachedQuizzes;

                var quizzes = await _unitOfWork.Quizzes.GetActiveQuizzesAsync(cancellationToken);
                await _cacheService.SetAsync(cacheKey, quizzes, TimeSpan.FromMinutes(15));

                return quizzes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active quizzes");
                throw new ServiceException("Error retrieving active quizzes", ex);
            }
        }

        public async Task<bool> IsUserEligibleForQuizAsync(string userId, int quizId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _unitOfWork.Quizzes.IsUserEligibleForQuizAsync(userId, quizId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking quiz eligibility for user {UserId} and quiz {QuizId}", userId, quizId);
                throw new ServiceException("Error checking quiz eligibility", ex);
            }
        }

        public async Task<Quiz> PublishQuizAsync(int quizId, CancellationToken cancellationToken = default)
        {
            try
            {
                var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Quiz), quizId);

                if (quiz.Status == QuizStatus.Published)
                    throw new ValidationException("Quiz is already published");

                ValidateQuizForPublishing(quiz);

                quiz.Status = QuizStatus.Published;
                quiz.LastModifiedAt = DateTimeOffset.UtcNow;

                await _unitOfWork.Quizzes.UpdateAsync(quiz, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheService.RemoveAsync($"quiz_{quizId}");
                await _cacheService.RemoveAsync("active_quizzes");

                _logger.LogInformation("Published quiz with ID {QuizId}", quizId);
                return quiz;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing quiz with ID {QuizId}", quizId);
                throw new ServiceException("Error publishing quiz", ex);
            }
        }

        public async Task<Quiz> UnpublishQuizAsync(int quizId, CancellationToken cancellationToken = default)
        {
            try
            {
                var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Quiz), quizId);

                if (quiz.Status != QuizStatus.Published)
                    throw new ValidationException("Quiz is not currently published");

                quiz.Status = QuizStatus.Draft;
                quiz.LastModifiedAt = DateTimeOffset.UtcNow;

                await _unitOfWork.Quizzes.UpdateAsync(quiz, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheService.RemoveAsync($"quiz_{quizId}");
                await _cacheService.RemoveAsync("active_quizzes");

                _logger.LogInformation("Unpublished quiz with ID {QuizId}", quizId);
                return quiz;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing quiz with ID {QuizId}", quizId);
                throw new ServiceException("Error unpublishing quiz", ex);
            }
        }

        public async Task<QuizStatistics> GetQuizStatisticsAsync(int quizId, CancellationToken cancellationToken = default)
        {
            try
            {
                var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Quiz), quizId);

                var attempts = await _unitOfWork.QuizAttempts.FindAsync(
                    qa => qa.QuizId == quizId && qa.Status == QuizAttemptStatus.Completed,
                    cancellationToken);

                var statistics = new QuizStatistics
                {
                    TotalAttempts = attempts.Count,
                    AverageScore = attempts.Any() ? attempts.Average(a => a.Score) : 0,
                    PassCount = attempts.Count(a => a.Score >= quiz.PassingScore),
                    FailCount = attempts.Count(a => a.Score < quiz.PassingScore),
                    AverageCompletionTime = attempts.Any()
                        ? TimeSpan.FromTicks((long)attempts.Average(a => a.Duration?.Ticks ?? 0))
                        : TimeSpan.Zero,
                    QuestionTypeDistribution = quiz.Questions
                        .GroupBy(q => q.Type)
                        .ToDictionary(g => g.Key, g => g.Count())
                };

                return statistics;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for quiz {QuizId}", quizId);
                throw new ServiceException("Error retrieving quiz statistics", ex);
            }
        }

        private static void ValidateQuiz(Quiz quiz)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(quiz.Title))
                errors.Add("Quiz title is required");

            if (string.IsNullOrWhiteSpace(quiz.Description))
                errors.Add("Quiz description is required");

            if (quiz.Duration <= TimeSpan.Zero)
                errors.Add("Quiz duration must be greater than zero");

            if (quiz.StartDate <= DateTimeOffset.UtcNow)
                errors.Add("Quiz start date must be in the future");

            if (quiz.EndDate.HasValue && quiz.EndDate <= quiz.StartDate)
                errors.Add("Quiz end date must be after start date");

            if (quiz.PassingScore < 0 || quiz.PassingScore > 100)
                errors.Add("Quiz passing score must be between 0 and 100");

            if (quiz.MaxAttempts <= 0)
                errors.Add("Maximum attempts must be greater than zero");

            if (errors.Any())
                throw new ValidationException($"Quiz validation failed: {string.Join(", ", errors)}");
        }

        private static void ValidateQuizForPublishing(Quiz quiz)
        {
            var errors = new List<string>();

            if (!quiz.Questions.Any())
                errors.Add("Quiz must have at least one question");

            if (quiz.Questions.Any(q => !q.Options.Any() && q.Type == QuestionType.MultipleChoice))
                errors.Add("All multiple choice questions must have options");

            if (quiz.Questions.Any(q => q.Points <= 0))
                errors.Add("All questions must have positive point values");

            if (errors.Any())
                throw new ValidationException($"Quiz cannot be published: {string.Join(", ", errors)}");
        }
    }
}
