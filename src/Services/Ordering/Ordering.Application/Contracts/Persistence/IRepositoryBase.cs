using System.Linq.Expressions;
using Ordering.Domain.Common;

namespace Ordering.Application.Contracts.Persistence
{
#nullable disable
    public interface IRepositoryBase<TEntity> where TEntity : EntityBase
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null, string include = null, bool disableTracking = true);
        Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includes = null, bool disableTracking = true);
        Task<TEntity> GetByIdAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task<TEntity> GetByIdAsync(Guid id);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
