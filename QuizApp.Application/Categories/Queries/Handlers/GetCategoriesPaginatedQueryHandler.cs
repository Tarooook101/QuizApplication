using MapsterMapper;
using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Specifications.Category;
using System.Linq.Expressions;


namespace QuizApp.Application.Categories.Queries.Handlers;
public class GetCategoriesPaginatedQueryHandler : IQueryHandler<GetCategoriesPaginatedQuery, PaginatedResult<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoriesPaginatedQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<CategoryDto>>> Handle(GetCategoriesPaginatedQuery request, CancellationToken cancellationToken)
    {
        var specification = new CategoriesPaginatedSpecificationSimple(
            request.PageNumber,
            request.PageSize,
            request.IsActive,
            request.SearchTerm);

        var categories = await _categoryRepository.GetAsync(specification, cancellationToken);

        // Build Expression<Func<Category, bool>> for CountAsync
        Expression<Func<Category, bool>>? filter = null;
        if (request.IsActive.HasValue && !string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            filter = c => c.IsActive == request.IsActive.Value &&
                          (c.Name.Contains(request.SearchTerm) || c.Description.Contains(request.SearchTerm));
        }
        else if (request.IsActive.HasValue)
        {
            filter = c => c.IsActive == request.IsActive.Value;
        }
        else if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            filter = c => c.Name.Contains(request.SearchTerm) || c.Description.Contains(request.SearchTerm);
        }

        var totalCount = await _categoryRepository.CountAsync(filter, cancellationToken);

        var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        var paginatedResult = PaginatedResult<CategoryDto>.Create(
            categoryDtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(paginatedResult);
    }
}
