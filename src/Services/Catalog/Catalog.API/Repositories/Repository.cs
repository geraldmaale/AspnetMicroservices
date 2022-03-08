using System.Linq.Expressions;
using Catalog.API.Data;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class Repository<T> : IRepository<T>
{
    public ICatalogContext Context { get; set; }

    public string? CollectionName { get; set; }

    public Repository(ICatalogContext context)
    {
        Context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var collection = Context.ConnectToMongo<T>(CollectionName);
        var results = await collection.Find(_ => true).ToListAsync();
        return results;
    }

    public async Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate)
    {
        var collection = Context.ConnectToMongo<T>(CollectionName);
        var result = await collection.Find(predicate).FirstOrDefaultAsync();
        return result;
    }

    public async Task<T> GetByNameAsync(FilterDefinition<T> predicate)
    {
        var collection = Context.ConnectToMongo<T>(CollectionName);
        var result = await collection.Find(predicate).FirstOrDefaultAsync();
        return result;
    }

    public async Task CreateAsync(T item)
    {
        var collection = Context.ConnectToMongo<T>(CollectionName);
        await collection.InsertOneAsync(item);
    }

    public async Task<bool> UpdateAsync(T item, Expression<Func<T, bool>> predicate)
    {
        var collection = Context.ConnectToMongo<T>(CollectionName);
        var result = await collection.ReplaceOneAsync(filter: predicate, replacement: item);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(FilterDefinition<T> predicate)
    {
        var collection = Context.ConnectToMongo<T>(CollectionName);

        var deleteResult = await collection.DeleteOneAsync(predicate);

        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }
}
