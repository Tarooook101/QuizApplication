using MapsterMapper;
using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Categories.Queries.Handlers;

public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Category> categories;

        if (request.IsActive.HasValue && request.IsActive.Value)
        {
            categories = await _categoryRepository.GetActiveCategoriesAsync(cancellationToken);
        }
        else
        {
            categories = await _categoryRepository.GetCategoriesOrderedAsync(cancellationToken);
        }

        var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        return Result.Success(categoryDtos);
    }
}