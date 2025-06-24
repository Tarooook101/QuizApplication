using MapsterMapper;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizAttempts.DTOs;
using QuizApp.Application.QuizAttempts.Queries;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.QuizAttempts.Handlers;

public class GetQuizAttemptByIdHandler : BaseHandler, IQueryHandler<GetQuizAttemptByIdQuery, QuizAttemptDto>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetQuizAttemptByIdHandler(
        IUnitOfWork unitOfWork,
        IQuizAttemptRepository quizAttemptRepository,
        ICurrentUserService currentUserService,
        IMapper mapper) : base(unitOfWork)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<QuizAttemptDto>> Handle(GetQuizAttemptByIdQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            return Result.Failure<QuizAttemptDto>("User not authenticated");

        var quizAttempt = await _quizAttemptRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quizAttempt == null)
            return Result.Failure<QuizAttemptDto>("Quiz attempt not found");

        if (quizAttempt.UserId != userId)
            return Result.Failure<QuizAttemptDto>("You can only view your own quiz attempts");

        var dto = _mapper.Map<QuizAttemptDto>(quizAttempt);
        return Result.Success(dto);
    }
}