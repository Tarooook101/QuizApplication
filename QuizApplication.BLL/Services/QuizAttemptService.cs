using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuizApplication.BLL.DTOs;
using QuizApplication.BLL.Interfaces;
using QuizApplication.DAL.Common;
using QuizApplication.DAL.Entities;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Services
{
    public class QuizAttemptService : IQuizAttemptService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILogger<QuizAttemptService> _logger;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

        public QuizAttemptService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<QuizAttemptService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<QuizAttempt?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"quiz_attempt_{id}";
            var cachedAttempt = await _cacheService.GetAsync<QuizAttempt>(cacheKey, cancellationToken);

            if (cachedAttempt != null)
                return cachedAttempt;

            var attempt = await _unitOfWork.QuizAttempts.GetByIdAsync(id, cancellationToken);

            if (attempt != null)
                await _cacheService.SetAsync(cacheKey, attempt, CacheDuration, cancellationToken);

            return attempt;
        }

        public async Task<IReadOnlyList<QuizAttempt>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.QuizAttempts.GetAllAsync(cancellationToken);
        }

        public async Task<QuizAttempt> CreateAsync(QuizAttempt entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var attempt = await _unitOfWork.QuizAttempts.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return attempt;
        }

        public async Task UpdateAsync(QuizAttempt entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _unitOfWork.QuizAttempts.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync($"quiz_attempt_{entity.Id}", cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var attempt = await GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(QuizAttempt), id);

            await _unitOfWork.QuizAttempts.DeleteAsync(attempt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync($"quiz_attempt_{id}", cancellationToken);
        }

        public async Task<QuizAttempt> StartQuizAttemptAsync(string userId, int quizId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            var canAttempt = await CanUserAttemptQuizAsync(userId, quizId, cancellationToken);
            if (!canAttempt)
                throw new ValidationException("User is not eligible to attempt this quiz");

            var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId, cancellationToken)
                ?? throw new NotFoundException(nameof(Quiz), quizId);

            var attempt = new QuizAttempt
            {
                UserId = userId,
                QuizId = quizId,
                StartedAt = DateTimeOffset.UtcNow,
                Status = QuizAttemptStatus.Started,
                CreatedBy = userId
            };

            return await CreateAsync(attempt, cancellationToken);
        }

        public async Task<QuizAttempt> SubmitQuizAttemptAsync(
         int attemptId,
         IEnumerable<QuestionResponse> responses,
         CancellationToken cancellationToken = default)
        {
            var attempt = await GetByIdAsync(attemptId, cancellationToken)
                ?? throw new NotFoundException(nameof(QuizAttempt), attemptId);

            if (attempt.Status == QuizAttemptStatus.Completed)
                throw new ValidationException("Quiz attempt has already been submitted");

            var quiz = await _unitOfWork.Quizzes.GetByIdAsync(attempt.QuizId, cancellationToken)
                ?? throw new NotFoundException(nameof(Quiz), attempt.QuizId);

            // Process responses in parallel for better performance
            var questionResponses = new ConcurrentBag<QuestionResponse>();
            var questions = await _unitOfWork.Questions.GetByQuizIdAsync(quiz.Id, true, cancellationToken);
            var totalScore = 0;

            await Parallel.ForEachAsync(responses, cancellationToken, async (response, ct) =>
            {
                var question = questions.FirstOrDefault(q => q.Id == response.QuestionId)
                    ?? throw new NotFoundException(nameof(Question), response.QuestionId);

                response.QuizAttemptId = attemptId;
                response.IsCorrect = await EvaluateResponseAsync(question, response.Response, ct);
                response.ScoreEarned = response.IsCorrect ? question.Points : 0;
                response.CreatedBy = attempt.UserId;

                totalScore += response.ScoreEarned;
                questionResponses.Add(response);
            });

            // Update attempt details
            attempt.Status = QuizAttemptStatus.Completed;
            attempt.CompletedAt = DateTimeOffset.UtcNow;
            attempt.Score = totalScore;
            attempt.LastModifiedBy = attempt.UserId;
            attempt.LastModifiedAt = DateTimeOffset.UtcNow;

            // Get the execution strategy
            var strategy = await _unitOfWork.CreateExecutionStrategy();

            // Execute the transaction with proper error handling
            await strategy.ExecuteAsync(async () =>
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                try
                {
                    await _unitOfWork.QuestionResponses.AddRangeAsync(questionResponses, cancellationToken);
                    await _unitOfWork.QuizAttempts.UpdateAsync(attempt, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    return attempt;
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    throw;
                }
            });

            return attempt;
        }

        public async Task<IReadOnlyList<QuizAttempt>> GetUserAttemptsAsync(
            string userId,
            int quizId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return await _unitOfWork.QuizAttempts.GetUserAttemptsAsync(userId, quizId, cancellationToken);
        }

        public async Task<QuizAttemptResult> GetAttemptResultAsync(int attemptId, CancellationToken cancellationToken = default)
        {
            var attempt = await GetByIdAsync(attemptId, cancellationToken)
                ?? throw new NotFoundException(nameof(QuizAttempt), attemptId);

            var responses = await _unitOfWork.QuestionResponses.GetByAttemptIdAsync(attemptId, cancellationToken);
            var quiz = await _unitOfWork.Quizzes.GetByIdAsync(attempt.QuizId, cancellationToken)
                ?? throw new NotFoundException(nameof(Quiz), attempt.QuizId);

            return new QuizAttemptResult
            {
                AttemptId = attemptId,
                Score = attempt.Score,
                IsPassed = attempt.Score >= quiz.PassingScore,
                TimeTaken = attempt.Duration ?? TimeSpan.Zero,
                CorrectAnswers = responses.Count(r => r.IsCorrect),
                TotalQuestions = responses.Count,
                Responses = responses.ToList()
            };
        }

        public async Task<bool> CanUserAttemptQuizAsync(string userId, int quizId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId, cancellationToken)
                ?? throw new NotFoundException(nameof(Quiz), quizId);

            if (!quiz.IsActive)
                return false;

            var attemptCount = await _unitOfWork.QuizAttempts.GetAttemptCountAsync(userId, quizId, cancellationToken);
            return quiz.CanAttempt(userId, attemptCount);
        }

        private async Task<bool> EvaluateResponseAsync(Question question, string? response, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            return question.Type switch
            {
                QuestionType.MultipleChoice or QuestionType.TrueFalse => await EvaluateMultipleChoiceAsync(question, response, cancellationToken),
                QuestionType.ShortAnswer => await EvaluateShortAnswerAsync(question, response, cancellationToken),
                _ => throw new NotSupportedException($"Question type {question.Type} is not supported for automatic evaluation")
            };
        }

        private async Task<bool> EvaluateMultipleChoiceAsync(Question question, string response, CancellationToken cancellationToken)
        {
            var correctOption = await _unitOfWork.Options
                .FirstOrDefaultAsync(o => o.QuestionId == question.Id && o.IsCorrect, cancellationToken);

            return correctOption?.Text.Equals(response, StringComparison.OrdinalIgnoreCase) ?? false;
        }

        private async Task<bool> EvaluateShortAnswerAsync(Question question, string response, CancellationToken cancellationToken)
        {
            var correctOption = await _unitOfWork.Options
                .FirstOrDefaultAsync(o => o.QuestionId == question.Id && o.IsCorrect, cancellationToken);

            return correctOption?.Text.Equals(response.Trim(), StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
