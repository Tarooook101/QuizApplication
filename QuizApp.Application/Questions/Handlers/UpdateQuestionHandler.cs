using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Questions.Commands;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Questions.Handlers;

public class UpdateQuestionHandler : BaseHandler, ICommandHandler<UpdateQuestionCommand>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateQuestionHandler(
        IUnitOfWork unitOfWork,
        IQuestionRepository questionRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _questionRepository = questionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (question == null)
            return Result.Failure("Question not found");

        var existingQuestionWithOrder = await _questionRepository.GetByQuizIdAndOrderAsync(
            question.QuizId, request.OrderIndex, cancellationToken);

        if (existingQuestionWithOrder != null && existingQuestionWithOrder.Id != request.Id)
            return Result.Failure("Another question with this order index already exists in the quiz");

        try
        {
            question.Update(
                request.Text,
                request.Type,
                request.Points,
                request.OrderIndex,
                request.IsRequired,
            request.Explanation,
                request.ImageUrl,
                _currentUserService.UserId);

            await _questionRepository.UpdateAsync(question, cancellationToken);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}