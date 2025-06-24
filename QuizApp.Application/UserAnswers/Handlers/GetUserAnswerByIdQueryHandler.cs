using MapsterMapper;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.UserAnswers.DTOs;
using QuizApp.Application.UserAnswers.Queries;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.UserAnswers.Handlers;

public class GetUserAnswerByIdQueryHandler : BaseHandler, IQueryHandler<GetUserAnswerByIdQuery, UserAnswerDetailsDto>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IMapper _mapper;

    public GetUserAnswerByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IUserAnswerRepository userAnswerRepository,
        IMapper mapper) : base(unitOfWork)
    {
        _userAnswerRepository = userAnswerRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserAnswerDetailsDto>> Handle(GetUserAnswerByIdQuery request, CancellationToken cancellationToken)
    {
        var userAnswer = await _userAnswerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (userAnswer == null)
            return Result.Failure<UserAnswerDetailsDto>("User answer not found");

        var result = _mapper.Map<UserAnswerDetailsDto>(userAnswer);
        return Result.Success(result);
    }
}