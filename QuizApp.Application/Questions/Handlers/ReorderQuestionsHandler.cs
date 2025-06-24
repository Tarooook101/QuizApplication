using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Questions.Commands;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Questions.Handlers;

public class ReorderQuestionsHandler : BaseHandler, ICommandHandler<ReorderQuestionsCommand>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ICurrentUserService _currentUserService;

    public ReorderQuestionsHandler(
        IUnitOfWork unitOfWork,
        IQuestionRepository questionRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _questionRepository = questionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ReorderQuestionsCommand request, CancellationToken cancellationToken)
    {
        var questions = await _questionRepository.GetByQuizIdAsync(request.QuizId, cancellationToken);
        var questionDict = questions.ToDictionary(q => q.Id);

        var orderIndexes = request.Questions.Select(q => q.OrderIndex).ToList();
        if (orderIndexes.Count != orderIndexes.Distinct().Count())
            return Result.Failure("Duplicate order indexes are not allowed");

        foreach (var questionOrder in request.Questions)
        {
            if (!questionDict.TryGetValue(questionOrder.Id, out var question))
                return Result.Failure($"Question with ID {questionOrder.Id} not found in the specified quiz");

            question.UpdateOrder(questionOrder.OrderIndex, _currentUserService.UserId);
            await _questionRepository.UpdateAsync(question, cancellationToken);
        }

        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}