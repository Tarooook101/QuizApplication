using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuizApplication.BLL.Interfaces;
using QuizApplication.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.BLL.Services
{
    public abstract class BaseService<TEntity, TKey> : IService<TEntity, TKey>
        where TEntity : class
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IRepository<TEntity, TKey> Repository;
        protected readonly ILogger<BaseService<TEntity, TKey>> Logger;

        protected BaseService(
            IUnitOfWork unitOfWork,
            ILogger<BaseService<TEntity, TKey>> logger)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            Repository = unitOfWork.Repository<TEntity, TKey>();
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.GetByIdAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving entity with ID {Id}", id);
                throw new ServiceException($"Error retrieving entity with ID {id}", ex);
            }
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.GetAllAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving all entities");
                throw new ServiceException("Error retrieving all entities", ex);
            }
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await ValidateEntityAsync(entity, cancellationToken);

                var strategy = await UnitOfWork.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    await UnitOfWork.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        var result = await Repository.AddAsync(entity, cancellationToken);
                        await UnitOfWork.SaveChangesAsync(cancellationToken);
                        await UnitOfWork.CommitTransactionAsync(cancellationToken);
                        return result;
                    }
                    catch
                    {
                        await UnitOfWork.RollbackTransactionAsync(cancellationToken);
                        throw;
                    }
                });
            }
            catch (ValidationException ex)
            {
                Logger.LogWarning(ex, "Validation failed for entity creation");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating entity");
                throw new ServiceException("Error creating entity", ex);
            }
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await ValidateEntityAsync(entity, cancellationToken);

                var strategy = await UnitOfWork.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await UnitOfWork.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        await Repository.UpdateAsync(entity, cancellationToken);
                        await UnitOfWork.SaveChangesAsync(cancellationToken);
                        await UnitOfWork.CommitTransactionAsync(cancellationToken);
                        return true;
                    }
                    catch
                    {
                        await UnitOfWork.RollbackTransactionAsync(cancellationToken);
                        throw;
                    }
                });
            }
            catch (ValidationException ex)
            {
                Logger.LogWarning(ex, "Validation failed for entity update");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating entity");
                throw new ServiceException("Error updating entity", ex);
            }
        }

        public virtual async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await GetByIdAsync(id, cancellationToken);
                if (entity == null)
                {
                    throw new NotFoundException($"Entity with ID {id} not found");
                }

                var strategy = await UnitOfWork.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await UnitOfWork.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        await Repository.DeleteAsync(entity, cancellationToken);
                        await UnitOfWork.SaveChangesAsync(cancellationToken);
                        await UnitOfWork.CommitTransactionAsync(cancellationToken);
                        return true;
                    }
                    catch
                    {
                        await UnitOfWork.RollbackTransactionAsync(cancellationToken);
                        throw;
                    }
                });
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting entity with ID {Id}", id);
                throw new ServiceException($"Error deleting entity with ID {id}", ex);
            }
        }

        protected virtual Task ValidateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ValidationException("Entity cannot be null");
            }
            return Task.CompletedTask;
        }

        protected async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await Repository.AnyAsync(predicate, cancellationToken);
        }
    }
}
