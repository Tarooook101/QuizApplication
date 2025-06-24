using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Common.Interfaces;


namespace QuizApp.Application.Categories.Queries;

public record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryDto>;
