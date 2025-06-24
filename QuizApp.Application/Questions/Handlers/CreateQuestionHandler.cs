using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Questions.Commands;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Questions.Handlers;

public class CreateQuestionHandler : BaseHandler, ICommandHandler<CreateQuestionCommand, Guid>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateQuestionHandler(
        IUnitOfWork unitOfWork,
        IQuestionRepository questionRepository,
        IQuizRepository quizRepository,
        ICurrentUserService currentUserService) : base(unitOfWork)
    {
        _questionRepository = questionRepository;
        _quizRepository = quizRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz == null)
            return Result.Failure<Guid>("Quiz not found");

        if (request.OrderIndex < 0)
        {
            var maxOrder = await _questionRepository.GetMaxOrderIndexByQuizIdAsync(request.QuizId, cancellationToken);
            request.OrderIndex = maxOrder + 1;
        }

        var existingQuestion = await _questionRepository.GetByQuizIdAndOrderAsync(
            request.QuizId, request.OrderIndex, cancellationToken);

        if (existingQuestion != null)
            return Result.Failure<Guid>("A question with this order index already exists in the quiz");

        try
        {
            var question = new Question(
                request.Text,
                request.Type,
                request.Points,
                request.OrderIndex,
                request.QuizId,
            request.IsRequired,
            request.Explanation,
                request.ImageUrl);

            question.MarkAsUpdated(_currentUserService.UserId);

            await _questionRepository.AddAsync(question, cancellationToken);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(question.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}