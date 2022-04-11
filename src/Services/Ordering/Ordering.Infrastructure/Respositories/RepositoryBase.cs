using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Common;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Respositories;

public class RepositoryBase<TEntity>: IAsyncRepository<TEntity> where TEntity:EntityBase
{
    protected readonly OrderDbContext DbContext;

    public RepositoryBase(OrderDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        var results = await DbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        return results;
    }

    public async Task<IReadOnlyList<TEntity>> GetByIdAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var results = await DbContext.Set<TEntity>().Where(predicate).ToListAsync();
        return results;
    }

    public async Task<IReadOnlyList<TEntity>> GetByIdAsync(Expression<Func<TEntity, 
            bool>> predicate = null, Func<IQueryable<TEntity>, 
            IOrderedQueryable<TEntity>> orderBy = null, string includeString = null,
        bool disableTracking = true)
    {
        IQueryable<TEntity> source = DbContext.Set<TEntity>();
        if (disableTracking)
            source = source.AsNoTracking();
        if (predicate != null)
            source = source.Where(predicate);
        if (orderBy != null)
            return await orderBy(source).ToListAsync();
        
        return await source.ToListAsync();
    }

    public async Task<List<TEntity>> GetByIdAsync(Expression<Func<TEntity,
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

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await DbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
        await DbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        DbContext.Entry(entity).State = EntityState.Modified;
        await DbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
        return DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(id);
        DbContext.Set<TEntity>().Remove(entity!);
        await DbContext.SaveChangesAsync();
    }
}