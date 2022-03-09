using MongoDB.Driver;

namespace Catalog.API.Data;

public class CatalogContext : ICatalogContext
{
    public const string CategoryCollection = "Category";
    public const string ProductCollection = "Product";

    private readonly IConfiguration _configuration;

    public IMongoCollection<T> ConnectToMongo<T>(string? collection)
    {
        var credentials = _configuration.GetValue<string>("DatabaseSettings:Credentials");
        var databaseConn = _configuration.GetValue<string>("DatabaseSettings:DatabaseName");
        var connectionStringWithCredentials = $"{credentials}/{databaseConn}/?authSource=admin";

        var clientWithCredentials = new MongoClient(connectionStringWithCredentials);
        var databaseWithCredentials = clientWithCredentials.GetDatabase(databaseConn);

        var collectionResults = databaseWithCredentials.GetCollection<T>(collection);
        return collectionResults;
    }

    public CatalogContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

}
