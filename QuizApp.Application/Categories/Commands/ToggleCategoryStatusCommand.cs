using QuizApp.Application.Categories.DTOs;
using QuizApp.Application.Common.Interfaces;


namespace QuizApp.Application.Categories.Commands;

public record ToggleCategoryStatusCommand(Guid Id) : ICommand<CategoryDto>;
