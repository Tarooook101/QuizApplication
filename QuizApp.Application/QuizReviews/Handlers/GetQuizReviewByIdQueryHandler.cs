using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizReviews.DTOs;
using QuizApp.Application.QuizReviews.Queries;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.QuizReviews.Handlers;


public class GetQuizReviewByIdQueryHandler : IQueryHandler<GetQuizReviewByIdQuery, QuizReviewDto>
{
    private readonly IQuizReviewRepository _quizReviewRepository;
    private readonly IMapper _mapper;

    public GetQuizReviewByIdQueryHandler(IQuizReviewRepository quizReviewRepository, IMapper mapper)
    {
        _quizReviewRepository = quizReviewRepository;
        _mapper = mapper;
    }

    public async Task<Result<QuizReviewDto>> Handle(GetQuizReviewByIdQuery request, CancellationToken cancellationToken)
    {
        var review = await _quizReviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
            return Result.Failure<QuizReviewDto>("Review not found");

        var reviewDto = _mapper.Map<QuizReviewDto>(review);
        return Result.Success(reviewDto);
    }
}