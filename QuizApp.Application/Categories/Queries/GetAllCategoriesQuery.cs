using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Common.Interfaces;


namespace QuizApp.Application.Categories.Queries;

public record GetAllCategoriesQuery(bool? IsActive = null) : IQuery<IEnumerable<CategoryDto>>;
