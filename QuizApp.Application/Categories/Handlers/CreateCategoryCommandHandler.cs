using MapsterMapper;
using QuizApp.Application.Categories.Commands;
using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Categories.Handlers;

public class CreateCategoryCommandHandler : BaseHandler, ICommandHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService,
        IMapper mapper) : base(unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existingCategory = await _categoryRepository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken);
        if (existingCategory)
        {
            return Result.Failure<CategoryDto>("A category with this name already exists");
        }

        var category = new Category(
            request.Name,
            request.Description,
            request.IconUrl,
        request.DisplayOrder,
        request.Color);

        if (_currentUserService.IsAuthenticated)
        {
            category.GetType().GetProperty("CreatedBy")?.SetValue(category, _currentUserService.UserId);
        }

        await _categoryRepository.AddAsync(category, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var categoryDto = _mapper.Map<CategoryDto>(category);
        return Result.Success(categoryDto);
    }
}
