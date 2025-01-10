using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuizApplication.BLL.DTOs;
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
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuestionService> _logger;
        private readonly ICacheService _cacheService;

        public QuestionService(
            IUnitOfWork unitOfWork,
            ILogger<QuestionService> logger,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Question?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = $"question_{id}";
                var cachedQuestion = await _cacheService.GetAsync<Question>(cacheKey, cancellationToken);
                if (cachedQuestion != null)
                    return cachedQuestion;

                var question = await _unitOfWork.Questions.GetByIdAsync(id, cancellationToken);
                if (question != null)
                    await _cacheService.SetAsync(cacheKey, question, TimeSpan.FromMinutes(30), cancellationToken);

                return question;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting question with ID: {QuestionId}", id);
                throw new ServiceException("Failed to retrieve question", ex);
            }
        }

        public async Task<IReadOnlyList<Question>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _unitOfWork.Questions.GetAllAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all questions");
                throw new ServiceException("Failed to retrieve questions", ex);
            }
        }

        public async Task<Question> CreateAsync(Question question, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateQuestion(question);

                var quiz = await _unitOfWork.Quizzes.GetByIdAsync(question.QuizId, cancellationToken)
                    ?? throw new NotFoundException("Quiz", question.QuizId);

                // Set display order to be last in the quiz if not specified
                if (question.DisplayOrder == 0)
                {
                    var lastQuestion = await _unitOfWork.Questions
                        .GetQueryable()
                        .Where(q => q.QuizId == question.QuizId)
                        .OrderByDescending(q => q.DisplayOrder)
                        .FirstOrDefaultAsync(cancellationToken);

                    question.DisplayOrder = (lastQuestion?.DisplayOrder ?? 0) + 1;
                }

                await _unitOfWork.Questions.AddAsync(question, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return question;
            }
            catch (Exception ex) when (ex is not ValidationException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error occurred while creating question for quiz: {QuizId}", question.QuizId);
                throw new ServiceException("Failed to create question", ex);
            }
        }

        public async Task UpdateAsync(Question question, CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateQuestion(question);

                var existingQuestion = await _unitOfWork.Questions.GetByIdAsync(question.Id, cancellationToken)
                    ?? throw new NotFoundException("Question", question.Id);

                // Update cache if exists
                var cacheKey = $"question_{question.Id}";
                await _cacheService.RemoveAsync(cacheKey, cancellationToken);

                await _unitOfWork.Questions.UpdateAsync(question, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error occurred while updating question: {QuestionId}", question.Id);
                throw new ServiceException("Failed to update question", ex);
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var question = await _unitOfWork.Questions.GetByIdAsync(id, cancellationToken)
                    ?? throw new NotFoundException("Question", id);

                var cacheKey = $"question_{id}";
                await _cacheService.RemoveAsync(cacheKey, cancellationToken);

                await _unitOfWork.Questions.DeleteAsync(question, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error occurred while deleting question: {QuestionId}", id);
                throw new ServiceException("Failed to delete question", ex);
            }
        }

        public async Task<IReadOnlyList<Question>> GetQuestionsByQuizAsync(int quizId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _unitOfWork.Questions.GetByQuizIdAsync(quizId, includeOptions: true, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting questions for quiz: {QuizId}", quizId);
                throw new ServiceException("Failed to retrieve quiz questions", ex);
            }
        }

        public async Task<Question> AddQuestionToQuizAsync(int quizId, Question question, CancellationToken cancellationToken = default)
        {
            try
            {
                question.QuizId = quizId;
                return await CreateAsync(question, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error occurred while adding question to quiz: {QuizId}", quizId);
                throw new ServiceException("Failed to add question to quiz", ex);
            }
        }

        public async Task UpdateQuestionOrderAsync(int quizId, Dictionary<int, int> questionOrders, CancellationToken cancellationToken = default)
        {
            try
            {
                var questions = await _unitOfWork.Questions.GetByQuizIdAsync(quizId, false, cancellationToken);
                var orderUpdates = questions
                    .Where(q => questionOrders.ContainsKey(q.Id))
                    .Select(q => (q.Id, questionOrders[q.Id]))
                    .ToList();

                await _unitOfWork.Questions.UpdateQuestionOrdersAsync(orderUpdates, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating question orders for quiz: {QuizId}", quizId);
                throw new ServiceException("Failed to update question orders", ex);
            }
        }

        public async Task<QuestionStatistics> GetQuestionStatisticsAsync(int questionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var question = await _unitOfWork.Questions.GetByIdAsync(questionId, cancellationToken)
                    ?? throw new NotFoundException("Question", questionId);

                var responses = await _unitOfWork.QuestionResponses
                    .FindAsync(r => r.QuestionId == questionId, cancellationToken);

                var statistics = new QuestionStatistics
                {
                    TotalResponses = responses.Count,
                    CorrectResponses = responses.Count(r => r.IsCorrect),
                    AverageResponseTime = responses.Any()
                        ? TimeSpan.FromTicks((long)responses.Average(r => r.TimeSpent?.Ticks ?? 0))
                        : TimeSpan.Zero
                };

                // Calculate answer distribution
                foreach (var response in responses.Where(r => r.Response != null))
                {
                    if (!statistics.AnswerDistribution.ContainsKey(response.Response!))
                        statistics.AnswerDistribution[response.Response!] = 0;
                    statistics.AnswerDistribution[response.Response!]++;
                }

                return statistics;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error occurred while getting statistics for question: {QuestionId}", questionId);
                throw new ServiceException("Failed to retrieve question statistics", ex);
            }
        }

        private static void ValidateQuestion(Question question)
        {
            if (string.IsNullOrWhiteSpace(question.Text))
                throw new ValidationException("Question text is required");

            if (question.Points < 0)
                throw new ValidationException("Question points cannot be negative");

            if (question.Type == QuestionType.MultipleChoice && !question.Options.Any())
                throw new ValidationException("Multiple choice questions must have at least one option");

            if (question.Type == QuestionType.MultipleChoice && !question.Options.Any(o => o.IsCorrect))
                throw new ValidationException("Multiple choice questions must have at least one correct option");

            if (question.TimeLimit.HasValue && question.TimeLimit.Value <= TimeSpan.Zero)
                throw new ValidationException("Question time limit must be positive");
        }
    }
}
