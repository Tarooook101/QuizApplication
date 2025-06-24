using QuizApp.Domain.Repositories;
namespace QuizApp.Application.Common.Helpers;

public abstract class BaseHandler
{
    protected readonly IUnitOfWork UnitOfWork;

    protected BaseHandler(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }
}