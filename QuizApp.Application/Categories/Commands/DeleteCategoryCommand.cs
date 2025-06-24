using QuizApp.Application.Common.Interfaces;


namespace QuizApp.Application.Categories.Commands;

public record DeleteCategoryCommand(Guid Id) : ICommand;
