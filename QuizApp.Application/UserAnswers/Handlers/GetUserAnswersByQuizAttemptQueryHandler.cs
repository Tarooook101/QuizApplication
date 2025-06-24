using MapsterMapper;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.UserAnswers.DTOs;
using QuizApp.Application.UserAnswers.Queries;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.UserAnswers.Handlers;

public class GetUserAnswersByQuizAttemptQueryHandler : BaseHandler, IQueryHandler<GetUserAnswersByQuizAttemptQuery, IEnumerable<UserAnswerDetailsDto>>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IMapper _mapper;

    public GetUserAnswersByQuizAttemptQueryHandler(
        IUnitOfWork unitOfWork,
        IUserAnswerRepository userAnswerRepository,
        IMapper mapper) : base(unitOfWork)
    {
        _userAnswerRepository = userAnswerRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<UserAnswerDetailsDto>>> Handle(GetUserAnswersByQuizAttemptQuery request, CancellationToken cancellationToken)
    {
        var userAnswers = await _userAnswerRepository.GetByQuizAttemptIdAsync(request.QuizAttemptId, cancellationToken);
        var result = _mapper.Map<IEnumerable<UserAnswerDetailsDto>>(userAnswers);
        return Result.Success(result);
    }
}