using System.Linq.Expressions;
using Catalog.API.Data;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public abstract class Repository<T> : IRepository<T>
{
    public IMongoCollection<T> MongoCollection { get; set; }

    public abstract string? CollectionName { get; set; }

    protected Repository(ICatalogContext context)
    {
        MongoCollection = context.ConnectToMongo<T>(CollectionName);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var results = await MongoCollection.Find(_ => true).ToListAsync();
        return results;
    }

    public async Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate)
    {
        var result = await MongoCollection.Find(predicate).FirstOrDefaultAsync();
        return result;
    }

    public async Task<T> GetByNameAsync(FilterDefinition<T> predicate)
    {
        var result = await MongoCollection.Find(predicate).FirstOrDefaultAsync();
        return result;
    }

    public async Task CreateAsync(T item)
    {
        await MongoCollection.InsertOneAsync(item);
    }

    public async Task<bool> UpdateAsync(T item, FilterDefinition<T> predicate)
    {
        var result = await MongoCollection.ReplaceOneAsync(filter: predicate, replacement: item);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(FilterDefinition<T> predicate)
    {
        var deleteResult = await MongoCollection.DeleteOneAsync(predicate);

        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }
}
