using MapsterMapper;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizAttempts.DTOs;
using QuizApp.Application.QuizAttempts.Queries;
using QuizApp.Application.QuizAttempts.Specifications;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.QuizAttempts.Handlers;

public class GetQuizAttemptsByQuizHandler : BaseHandler, IQueryHandler<GetQuizAttemptsByQuizQuery, PaginatedResult<QuizAttemptDto>>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IMapper _mapper;

    public GetQuizAttemptsByQuizHandler(
        IUnitOfWork unitOfWork,
        IQuizAttemptRepository quizAttemptRepository,
        IMapper mapper) : base(unitOfWork)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<QuizAttemptDto>>> Handle(GetQuizAttemptsByQuizQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _quizAttemptRepository.CountAsync(qa => qa.QuizId == request.QuizId, cancellationToken);

        var spec = new QuizAttemptsByQuizSpecification(request.QuizId, request.Pagination.Skip, request.Pagination.Take);
        var quizAttempts = await _quizAttemptRepository.GetAsync(spec, cancellationToken);

        var dtos = _mapper.Map<IEnumerable<QuizAttemptDto>>(quizAttempts);
        var result = PaginatedResult<QuizAttemptDto>.Create(dtos, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);

        return Result.Success(result);
    }
}