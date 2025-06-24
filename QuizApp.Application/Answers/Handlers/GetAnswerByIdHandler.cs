using MapsterMapper;
using QuizApp.Application.Answers.DTOs;
using QuizApp.Application.Answers.Queries;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Answers.Handlers;

public class GetAnswerByIdHandler : BaseHandler, IQueryHandler<GetAnswerByIdQuery, AnswerDto>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IMapper _mapper;

    public GetAnswerByIdHandler(
        IUnitOfWork unitOfWork,
        IAnswerRepository answerRepository,
        IMapper mapper) : base(unitOfWork)
    {
        _answerRepository = answerRepository;
        _mapper = mapper;
    }

    public async Task<Result<AnswerDto>> Handle(GetAnswerByIdQuery request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (answer == null)
            return Result.Failure<AnswerDto>("Answer not found");

        var answerDto = _mapper.Map<AnswerDto>(answer);
        return Result.Success(answerDto);
    }
}