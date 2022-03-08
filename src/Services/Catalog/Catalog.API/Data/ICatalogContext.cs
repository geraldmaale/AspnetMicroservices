using MongoDB.Driver;

namespace Catalog.API.Data;

public interface ICatalogContext
{
    IMongoCollection<T> ConnectToMongo<T>(string? collection);
}