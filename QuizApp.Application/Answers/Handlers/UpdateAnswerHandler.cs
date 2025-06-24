using MapsterMapper;
using QuizApp.Application.Answers.Commands;
using QuizApp.Application.Answers.DTOs;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Answers.Handlers;

public class UpdateAnswerHandler : BaseHandler, ICommandHandler<UpdateAnswerCommand, AnswerDto>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateAnswerHandler(
        IUnitOfWork unitOfWork,
        IAnswerRepository answerRepository,
        ICurrentUserService currentUserService,
        IMapper mapper) : base(unitOfWork)
    {
        _answerRepository = answerRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<AnswerDto>> Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _answerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (answer == null)
            return Result.Failure<AnswerDto>("Answer not found");
        answer.Update(
        request.Text,
            request.IsCorrect,
            request.OrderIndex,
            request.Explanation,
            _currentUserService.UserId);

        await _answerRepository.UpdateAsync(answer, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var answerDto = _mapper.Map<AnswerDto>(answer);
        return Result.Success(answerDto);
    }
}
