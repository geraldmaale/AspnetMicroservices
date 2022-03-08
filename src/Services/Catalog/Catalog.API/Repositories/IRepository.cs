using System.Linq.Expressions;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetByNameAsync(FilterDefinition<T> predicate);
    Task CreateAsync(T item);
    Task<bool> UpdateAsync(T item, Expression<Func<T, bool>> predicate);
    Task<bool> DeleteAsync(FilterDefinition<T> predicate);
}
