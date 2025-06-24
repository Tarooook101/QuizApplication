using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Questions.DTOs;
using QuizApp.Application.Questions.Queries;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Questions.Handlers;

public class GetQuestionByIdHandler : IQueryHandler<GetQuestionByIdQuery, QuestionDto>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public GetQuestionByIdHandler(IQuestionRepository questionRepository, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    public async Task<Result<QuestionDto>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (question == null)
            return Result.Failure<QuestionDto>("Question not found");

        var questionDto = _mapper.Map<QuestionDto>(question);
        return Result.Success(questionDto);
    }
}
