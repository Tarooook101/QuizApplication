using MapsterMapper;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.UserAnswers.Commands;
using QuizApp.Application.UserAnswers.DTOs;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.UserAnswers.Handlers;

public class GradeUserAnswerCommandHandler : BaseHandler, ICommandHandler<GradeUserAnswerCommand, UserAnswerDto>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GradeUserAnswerCommandHandler(
        IUnitOfWork unitOfWork,
        IUserAnswerRepository userAnswerRepository,
        ICurrentUserService currentUserService,
        IMapper mapper) : base(unitOfWork)
    {
        _userAnswerRepository = userAnswerRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<UserAnswerDto>> Handle(GradeUserAnswerCommand request, CancellationToken cancellationToken)
    {
        var userAnswer = await _userAnswerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (userAnswer == null)
            return Result.Failure<UserAnswerDto>("User answer not found");

        if (request.IsCorrect)
        {
            userAnswer.MarkAsCorrect(request.PointsEarned, _currentUserService.UserId);
        }
        else
        {
            userAnswer.MarkAsIncorrect(_currentUserService.UserId);
        }

        await _userAnswerRepository.UpdateAsync(userAnswer, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<UserAnswerDto>(userAnswer);
        return Result.Success(result);
    }
}