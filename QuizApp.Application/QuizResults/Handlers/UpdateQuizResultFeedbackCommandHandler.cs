using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizResults.Commands;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.QuizResults.Handlers;

public class UpdateQuizResultFeedbackCommandHandler : BaseHandler, ICommandHandler<UpdateQuizResultFeedbackCommand>
{
    private readonly IQuizResultRepository _quizResultRepository;

    public UpdateQuizResultFeedbackCommandHandler(
        IUnitOfWork unitOfWork,
        IQuizResultRepository quizResultRepository) : base(unitOfWork)
    {
        _quizResultRepository = quizResultRepository;
    }

    public async Task<Result> Handle(UpdateQuizResultFeedbackCommand request, CancellationToken cancellationToken)
    {
        var quizResult = await _quizResultRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quizResult == null)
            return Result.Failure("Quiz result not found");

        quizResult.UpdateFeedback(request.Feedback);

        await _quizResultRepository.UpdateAsync(quizResult, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
