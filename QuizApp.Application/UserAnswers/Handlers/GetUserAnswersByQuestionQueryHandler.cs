using MapsterMapper;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.UserAnswers.DTOs;
using QuizApp.Application.UserAnswers.Queries;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.UserAnswers.Handlers;

public class GetUserAnswersByQuestionQueryHandler : BaseHandler, IQueryHandler<GetUserAnswersByQuestionQuery, IEnumerable<UserAnswerDto>>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IMapper _mapper;

    public GetUserAnswersByQuestionQueryHandler(
        IUnitOfWork unitOfWork,
        IUserAnswerRepository userAnswerRepository,
        IMapper mapper) : base(unitOfWork)
    {
        _userAnswerRepository = userAnswerRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<UserAnswerDto>>> Handle(GetUserAnswersByQuestionQuery request, CancellationToken cancellationToken)
    {
        var userAnswers = await _userAnswerRepository.GetByQuestionIdAsync(request.QuestionId, cancellationToken);
        var result = _mapper.Map<IEnumerable<UserAnswerDto>>(userAnswers);
        return Result.Success(result);
    }
}