using MapsterMapper;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.UserAnswers.Commands;
using QuizApp.Application.UserAnswers.DTOs;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.UserAnswers.Handlers;

public class UpdateUserAnswerCommandHandler : BaseHandler, ICommandHandler<UpdateUserAnswerCommand, UserAnswerDto>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IAnswerRepository _answerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateUserAnswerCommandHandler(
        IUnitOfWork unitOfWork,
        IUserAnswerRepository userAnswerRepository,
        IAnswerRepository answerRepository,
        ICurrentUserService currentUserService,
        IMapper mapper) : base(unitOfWork)
    {
        _userAnswerRepository = userAnswerRepository;
        _answerRepository = answerRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<UserAnswerDto>> Handle(UpdateUserAnswerCommand request, CancellationToken cancellationToken)
    {
        var userAnswer = await _userAnswerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (userAnswer == null)
            return Result.Failure<UserAnswerDto>("User answer not found");

        if (request.SelectedAnswerId.HasValue)
        {
            var selectedAnswer = await _answerRepository.GetByIdAsync(request.SelectedAnswerId.Value, cancellationToken);
            if (selectedAnswer == null || selectedAnswer.QuestionId != userAnswer.QuestionId)
                return Result.Failure<UserAnswerDto>("Selected answer is not valid for this question");
        }
        userAnswer.UpdateAnswer(
            request.SelectedAnswerId,
            request.TextAnswer,
            request.TimeSpent,
            _currentUserService.UserId);

        await _userAnswerRepository.UpdateAsync(userAnswer, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<UserAnswerDto>(userAnswer);
        return Result.Success(result);
    }
}