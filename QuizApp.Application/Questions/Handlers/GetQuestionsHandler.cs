using MapsterMapper;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Application.Questions.DTOs;
using QuizApp.Application.Questions.Queries;
using QuizApp.Application.Questions.Specifications;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Questions.Handlers;

public class GetQuestionsHandler : IQueryHandler<GetQuestionsQuery, PaginatedResult<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public GetQuestionsHandler(IQuestionRepository questionRepository, IMapper mapper)
    {
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<QuestionDto>>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetQuestionsSpecification(request);
        var questions = await _questionRepository.GetAsync(specification, cancellationToken);
        var totalCount = await _questionRepository.CountAsync(new GetQuestionsCountSpecification(request), cancellationToken);

        var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);
        var paginatedResult = PaginatedResult<QuestionDto>.Create(
            questionDtos, totalCount, request.Pagination.PageNumber, request.Pagination.PageSize);

        return Result.Success(paginatedResult);
    }
}
