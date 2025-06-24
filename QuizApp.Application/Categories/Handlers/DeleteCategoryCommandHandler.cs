using QuizApp.Application.Categories.Commands;
using QuizApp.Application.Common.Helpers;
using QuizApp.Application.Common.Interfaces;
using QuizApp.Application.Common.Models;
using QuizApp.Domain.Repositories;


namespace QuizApp.Application.Categories.Handlers;

public class DeleteCategoryCommandHandler : BaseHandler, ICommandHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository) : base(unitOfWork)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result.Failure("Category not found");
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}