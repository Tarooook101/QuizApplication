using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizResults.DTOs;
using QuizApp.Application.QuizResults.Queries;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.QuizResults.Handlers;

public class GetQuizResultsByQuizQueryHandler : IQueryHandler<GetQuizResultsByQuizQuery, PaginatedResult<QuizResultDetailDto>>
{
    private readonly IQuizResultRepository _quizResultRepository;
    private readonly IMapper _mapper;

    public GetQuizResultsByQuizQueryHandler(IQuizResultRepository quizResultRepository, IMapper mapper)
    {
        _quizResultRepository = quizResultRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<QuizResultDetailDto>>> Handle(GetQuizResultsByQuizQuery request, CancellationToken cancellationToken)
    {
        var results = await _quizResultRepository.GetByQuizIdAsync(request.QuizId, cancellationToken);
        var totalCount = results.Count();

        var paginatedResults = results
            .Skip(request.Pagination.Skip)
            .Take(request.Pagination.Take)
            .ToList();

        var dtos = _mapper.Map<IEnumerable<QuizResultDetailDto>>(paginatedResults);

        var paginatedResult = PaginatedResult<QuizResultDetailDto>.Create(
            dtos, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);

        return Result.Success(paginatedResult);
    }
}