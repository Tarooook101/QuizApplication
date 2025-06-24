using MapsterMapper;
using QuizApp.Application.Answers.Commands;
using QuizApp.Application.Answers.DTOs;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Answers.Handlers;

public class CreateAnswerHandler : BaseHandler, ICommandHandler<CreateAnswerCommand, AnswerDto>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public CreateAnswerHandler(
        IUnitOfWork unitOfWork,
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository,
        IMapper mapper) : base(unitOfWork)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    public async Task<Result<AnswerDto>> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question == null)
            return Result.Failure<AnswerDto>("Question not found");

        var answer = new Answer(
            request.Text,
            request.IsCorrect,
            request.OrderIndex,
            request.QuestionId,
            request.Explanation);

        await _answerRepository.AddAsync(answer, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var answerDto = _mapper.Map<AnswerDto>(answer);
        return Result.Success(answerDto);
    }
}

