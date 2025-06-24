using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Quizzes.DTOs;
using QuizApp.Application.Quizzes.Specifications;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Quizzes.Queries;

public class GetQuizzesQueryHandler : IQueryHandler<GetQuizzesQuery, PaginatedResult<QuizDto>>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IMapper _mapper;

    public GetQuizzesQueryHandler(IQuizRepository quizRepository, IMapper mapper)
    {
        _quizRepository = quizRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<QuizDto>>> Handle(GetQuizzesQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetQuizzesSpecification(request);

        var quizzes = await _quizRepository.GetAsync(specification, cancellationToken);
        var totalCount = await _quizRepository.CountAsync(new GetQuizzesCountSpecification(request), cancellationToken);

        var quizDtos = _mapper.Map<IEnumerable<QuizDto>>(quizzes);

        var paginatedResult = PaginatedResult<QuizDto>.Create(
            quizDtos,
            totalCount,
            request.Pagination.PageNumber,
            request.Pagination.PageSize);

        return Result.Success(paginatedResult);
    }
}