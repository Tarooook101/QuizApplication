using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Questions.DTOs;
using QuizApp.Application.Questions.Queries;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Questions.Handlers;

public class GetQuestionsByQuizIdHandler : IQueryHandler<GetQuestionsByQuizIdQuery, IEnumerable<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public GetQuestionsByQuizIdHandler(IQuestionRepository questionRepository, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<QuestionDto>>> Handle(GetQuestionsByQuizIdQuery request, CancellationToken cancellationToken)
    {
        var questions = request.OrderByIndex
            ? await _questionRepository.GetByQuizIdOrderedAsync(request.QuizId, cancellationToken)
            : await _questionRepository.GetByQuizIdAsync(request.QuizId, cancellationToken);

        var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);
        return Result.Success(questionDtos);
    }
}