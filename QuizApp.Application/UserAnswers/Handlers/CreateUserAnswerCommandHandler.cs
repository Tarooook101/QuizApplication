using MapsterMapper;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.UserAnswers.Commands;
using QuizApp.Application.UserAnswers.DTOs;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.UserAnswers.Handlers;

public class CreateUserAnswerCommandHandler : BaseHandler, ICommandHandler<CreateUserAnswerCommand, UserAnswerDto>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IAnswerRepository _answerRepository;
    private readonly IMapper _mapper;

    public CreateUserAnswerCommandHandler(
        IUnitOfWork unitOfWork,
        IUserAnswerRepository userAnswerRepository,
        IQuizAttemptRepository quizAttemptRepository,
        IQuestionRepository questionRepository,
        IAnswerRepository answerRepository,
        IMapper mapper) : base(unitOfWork)
    {
        _userAnswerRepository = userAnswerRepository;
        _quizAttemptRepository = quizAttemptRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserAnswerDto>> Handle(CreateUserAnswerCommand request, CancellationToken cancellationToken)
    {
        var quizAttempt = await _quizAttemptRepository.GetByIdAsync(request.QuizAttemptId, cancellationToken);
        if (quizAttempt == null)
            return Result.Failure<UserAnswerDto>("Quiz attempt not found");

        var question = await _questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question == null)
            return Result.Failure<UserAnswerDto>("Question not found");

        if (request.SelectedAnswerId.HasValue)
        {
            var selectedAnswer = await _answerRepository.GetByIdAsync(request.SelectedAnswerId.Value, cancellationToken);
            if (selectedAnswer == null || selectedAnswer.QuestionId != request.QuestionId)
                return Result.Failure<UserAnswerDto>("Selected answer is not valid for this question");
        }

        var existingAnswer = await _userAnswerRepository.GetByQuizAttemptAndQuestionAsync(request.QuizAttemptId, request.QuestionId, cancellationToken);
        if (existingAnswer != null)
            return Result.Failure<UserAnswerDto>("User has already answered this question");
        var userAnswer = new UserAnswer(
            request.QuizAttemptId,
            request.QuestionId,
            request.SelectedAnswerId,
            request.TextAnswer,
            request.TimeSpent);

        await _userAnswerRepository.AddAsync(userAnswer, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<UserAnswerDto>(userAnswer);
        return Result.Success(result);
    }
}