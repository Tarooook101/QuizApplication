using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizResults.DTOs;
using QuizApp.Application.QuizResults.Queries;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.QuizResults.Handlers;

public class GetQuizResultsByUserQueryHandler : IQueryHandler<GetQuizResultsByUserQuery, PaginatedResult<QuizResultSummaryDto>>
{
    private readonly IQuizResultRepository _quizResultRepository;
    private readonly IMapper _mapper;

    public GetQuizResultsByUserQueryHandler(IQuizResultRepository quizResultRepository, IMapper mapper)
    {
        _quizResultRepository = quizResultRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<QuizResultSummaryDto>>> Handle(GetQuizResultsByUserQuery request, CancellationToken cancellationToken)
    {
        var results = await _quizResultRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var totalCount = results.Count();

        var paginatedResults = results
            .Skip(request.Pagination.Skip)
            .Take(request.Pagination.Take)
            .ToList();

        var dtos = _mapper.Map<IEnumerable<QuizResultSummaryDto>>(paginatedResults);

        var paginatedResult = PaginatedResult<QuizResultSummaryDto>.Create(
            dtos, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);

        return Result.Success(paginatedResult);
    }
}