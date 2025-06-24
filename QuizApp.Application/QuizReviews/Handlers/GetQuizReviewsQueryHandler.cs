using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizReviews.DTOs;
using QuizApp.Application.QuizReviews.Queries;
using QuizApp.Application.QuizReviews.Specifications;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.QuizReviews.Handlers;

public class GetQuizReviewsQueryHandler : IQueryHandler<GetQuizReviewsQuery, PaginatedResult<QuizReviewDto>>
{
    private readonly IQuizReviewRepository _quizReviewRepository;
    private readonly IMapper _mapper;

    public GetQuizReviewsQueryHandler(IQuizReviewRepository quizReviewRepository, IMapper mapper)
    {
        _quizReviewRepository = quizReviewRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<QuizReviewDto>>> Handle(GetQuizReviewsQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetQuizReviewsSpecification(
            request.QuizId,
            request.UserId,
            request.IsPublic,
            request.MinRating,
            request.MaxRating,
            request.Pagination.Skip,
            request.Pagination.Take);

        var reviews = await _quizReviewRepository.GetAsync(specification, cancellationToken);
        var totalCount = await _quizReviewRepository.CountAsync(specification, cancellationToken);

        var reviewDtos = _mapper.Map<IEnumerable<QuizReviewDto>>(reviews);

        var result = PaginatedResult<QuizReviewDto>.Create(
            reviewDtos,
            totalCount,
            request.Pagination.PageNumber,
            request.Pagination.PageSize);

        return Result.Success(result);
    }
}