using MapsterMapper;
using QuizApp.Application.Categories.Commands;
using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Categories.Handlers;

public class ToggleCategoryStatusCommandHandler : BaseHandler, ICommandHandler<ToggleCategoryStatusCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ToggleCategoryStatusCommandHandler(
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService,
        IMapper mapper) : base(unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(ToggleCategoryStatusCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result.Failure<CategoryDto>("Category not found");
        }

        if (category.IsActive)
        {
            category.Deactivate(_currentUserService.UserId);
        }
        else
        {
            category.Activate(_currentUserService.UserId);
        }

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var categoryDto = _mapper.Map<CategoryDto>(category);
        return Result.Success(categoryDto);
    }
}