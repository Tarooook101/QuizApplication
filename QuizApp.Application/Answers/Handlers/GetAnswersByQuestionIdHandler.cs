using MapsterMapper;
using QuizApp.Application.Answers.DTOs;
using QuizApp.Application.Answers.Queries;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Answers.Handlers;

public class GetAnswersByQuestionIdHandler : BaseHandler, IQueryHandler<GetAnswersByQuestionIdQuery, IEnumerable<AnswerDto>>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IMapper _mapper;

    public GetAnswersByQuestionIdHandler(
        IUnitOfWork unitOfWork,
        IAnswerRepository answerRepository,
        IMapper mapper) : base(unitOfWork)
    {
        _answerRepository = answerRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AnswerDto>>> Handle(GetAnswersByQuestionIdQuery request, CancellationToken cancellationToken)
    {
        var answers = await _answerRepository.GetByQuestionIdOrderedAsync(request.QuestionId, cancellationToken);
        var answerDtos = _mapper.Map<IEnumerable<AnswerDto>>(answers);
        return Result.Success(answerDtos);
    }
}
