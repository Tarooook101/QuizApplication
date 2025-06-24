using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizResults.Commands;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.QuizResults.Handlers;

public class CreateQuizResultCommandHandler : BaseHandler, ICommandHandler<CreateQuizResultCommand, Guid>
{
    private readonly IQuizResultRepository _quizResultRepository;

    public CreateQuizResultCommandHandler(
        IUnitOfWork unitOfWork,
        IQuizResultRepository quizResultRepository) : base(unitOfWork)
    {
        _quizResultRepository = quizResultRepository;
    }

    public async Task<Result<Guid>> Handle(CreateQuizResultCommand request, CancellationToken cancellationToken)
    {
        var existingResult = await _quizResultRepository.GetByQuizAttemptIdAsync(
            request.QuizAttemptId, cancellationToken);

        if (existingResult != null)
            return Result.Failure<Guid>("Quiz result already exists for this attempt");

        var quizResult = new QuizResult(
            request.QuizAttemptId,
            request.UserId,
            request.QuizId,
            request.Score,
            request.MaxScore,
            request.CorrectAnswers,
            request.TotalQuestions,
            request.TimeSpent,
        request.PassingThreshold);

        if (!string.IsNullOrEmpty(request.Feedback))
        {
            quizResult.UpdateFeedback(request.Feedback);
        }

        await _quizResultRepository.AddAsync(quizResult, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(quizResult.Id);
    }
}