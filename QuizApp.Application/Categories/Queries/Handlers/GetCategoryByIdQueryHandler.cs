using MapsterMapper;
using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Categories.Queries.Handlers;

public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result.Failure<CategoryDto>("Category not found");
        }

        var categoryDto = _mapper.Map<CategoryDto>(category);
        return Result.Success(categoryDto);
    }
}