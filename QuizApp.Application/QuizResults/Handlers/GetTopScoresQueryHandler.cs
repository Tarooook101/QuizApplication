using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizResults.DTOs;
using QuizApp.Application.QuizResults.Queries;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.QuizResults.Handlers;

public class GetTopScoresQueryHandler : IQueryHandler<GetTopScoresQuery, IEnumerable<QuizResultDetailDto>>
{
    private readonly IQuizResultRepository _quizResultRepository;
    private readonly IMapper _mapper;

    public GetTopScoresQueryHandler(IQuizResultRepository quizResultRepository, IMapper mapper)
    {
        _quizResultRepository = quizResultRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<QuizResultDetailDto>>> Handle(GetTopScoresQuery request, CancellationToken cancellationToken)
    {
        var topScores = await _quizResultRepository.GetTopScoresAsync(request.QuizId, request.Count, cancellationToken);
        var dtos = _mapper.Map<IEnumerable<QuizResultDetailDto>>(topScores);
        return Result.Success(dtos);
    }
}