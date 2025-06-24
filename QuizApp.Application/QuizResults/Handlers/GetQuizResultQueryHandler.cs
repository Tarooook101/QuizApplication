using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.QuizResults.DTOs;
using QuizApp.Application.QuizResults.Queries;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.QuizResults.Handlers;

public class GetQuizResultQueryHandler : IQueryHandler<GetQuizResultQuery, QuizResultDto>
{
    private readonly IQuizResultRepository _quizResultRepository;
    private readonly IMapper _mapper;

    public GetQuizResultQueryHandler(IQuizResultRepository quizResultRepository, IMapper mapper)
    {
        _quizResultRepository = quizResultRepository;
        _mapper = mapper;
    }

    public async Task<Result<QuizResultDto>> Handle(GetQuizResultQuery request, CancellationToken cancellationToken)
    {
        var quizResult = await _quizResultRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quizResult == null)
            return Result.Failure<QuizResultDto>("Quiz result not found");

        var dto = _mapper.Map<QuizResultDto>(quizResult);
        return Result.Success(dto);
    }
}
