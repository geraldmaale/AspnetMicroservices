using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Common;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Respositories;

#nullable disable

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{
    protected readonly OrderDbContext DbContext;

    public RepositoryBase(OrderDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        var results = await DbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
        return results;
    }

    public async Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        var results = await DbContext.Set<TEntity>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        return results;
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity,
            bool>> predicate = null, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null, string include = null,
        bool disableTracking = true)
    {
        IQueryable<TEntity> query = DbContext.Set<TEntity>();
        if (disableTracking)
            query = query.AsNoTracking();
        if (predicate != null)
            query = query.Where(predicate);
        if (!string.IsNullOrWhiteSpace(include))
            query = query.Include(include);
        if (orderBy != null)
            return await orderBy(query).ToListAsync();

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity,
        bool>> predicate = null, Func<IQueryable<TEntity>,
        IOrderedQueryable<TEntity>> orderBy = null,
        List<Expression<Func<TEntity, object>>> includes = null,
        bool disableTracking = true)
    {
        IQueryable<TEntity> source = DbContext.Set<TEntity>();
        if (disableTracking)
            source = source.AsNoTracking();
        if (includes != null)
            source = includes.Aggregate(source, (current, include) => current.Include(include));
        if (predicate != null)
            source = source.Where(predicate);
        if (orderBy != null)
            return await orderBy(source).ToListAsync();

        return await source.ToListAsync();
    }

    public async Task<TEntity> GetByIdAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        var results = await DbContext.Set<TEntity>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
        return results;
    }

    public async Task<TEntity> GetByIdAsync(Guid id)
    {
        return await DbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbContext.Set<TEntity>().Add(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbContext.Entry(entity).State = EntityState.Modified;
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbContext.Set<TEntity>().Remove(entity);
        return DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(id);
        DbContext.Set<TEntity>().Remove(entity!);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}