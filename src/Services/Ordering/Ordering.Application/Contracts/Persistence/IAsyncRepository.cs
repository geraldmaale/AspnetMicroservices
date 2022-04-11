using System.Linq.Expressions;
using Ordering.Domain.Common;

namespace Ordering.Application.Contracts.Persistence
{
    public interface IAsyncRepository<TEntity> where TEntity : EntityBase
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<IReadOnlyList<TEntity>> GetByIdAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IReadOnlyList<TEntity>> GetByIdAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeString = null,
            bool disableTracking = true);
        Task<List<TEntity>> GetByIdAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            bool disableTracking = true);
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(Guid id);
    }
}
