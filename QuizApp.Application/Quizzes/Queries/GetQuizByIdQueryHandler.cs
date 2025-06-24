using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Quizzes.DTOs;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Quizzes.Queries;

public class GetQuizByIdQueryHandler : IQueryHandler<GetQuizByIdQuery, QuizDto>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IMapper _mapper;

    public GetQuizByIdQueryHandler(IQuizRepository quizRepository, IMapper mapper)
    {
        _quizRepository = quizRepository;
        _mapper = mapper;
    }

    public async Task<Result<QuizDto>> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quiz == null)
        {
            return Result.Failure<QuizDto>("Quiz not found");
        }

        var quizDto = _mapper.Map<QuizDto>(quiz);
        return Result.Success(quizDto);
    }
}