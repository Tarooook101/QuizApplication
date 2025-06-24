using MapsterMapper;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.UserAnswers.DTOs;
using QuizApp.Application.UserAnswers.Queries;
using QuizApp.Application.UserAnswers.Specifications;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.UserAnswers.Handlers;

public class GetUserAnswersQueryHandler : BaseHandler, IQueryHandler<GetUserAnswersQuery, PaginatedResult<UserAnswerDto>>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IMapper _mapper;

    public GetUserAnswersQueryHandler(
        IUnitOfWork unitOfWork,
        IUserAnswerRepository userAnswerRepository,
        IMapper mapper) : base(unitOfWork)
    {
        _userAnswerRepository = userAnswerRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<UserAnswerDto>>> Handle(GetUserAnswersQuery request, CancellationToken cancellationToken)
    {
        var specification = new UserAnswerSpecification(
            request.QuizAttemptId,
            request.QuestionId,
            request.IsCorrect,
            request.PageNumber,
            request.PageSize);

        var userAnswers = await _userAnswerRepository.GetAsync(specification, cancellationToken);
        var totalCount = await _userAnswerRepository.CountAsync(specification, cancellationToken);

        var mappedUserAnswers = _mapper.Map<IEnumerable<UserAnswerDto>>(userAnswers);
        var result = PaginatedResult<UserAnswerDto>.Create(mappedUserAnswers, totalCount, request.PageNumber, request.PageSize);

        return Result.Success(result);
    }
}
