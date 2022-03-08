using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Data;

public class CatalogContext : ICatalogContext
{
    private readonly IConfiguration _configuration;

    public CatalogContext(IConfiguration configuration)
    {
        _configuration = configuration;

        // connect to the database with username and password
        var credentials = configuration.GetValue<string>("DatabaseSettings:Credentials");
        var databaseConn = configuration.GetValue<string>("DatabaseSettings:DatabaseName");
        var connectionStringWithCredentials = $"{credentials}/{databaseConn}/?authSource=admin";
        var clientWithCredentials = new MongoClient(connectionStringWithCredentials);
        var databaseWithCredentials = clientWithCredentials.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

        // No credentials
        var connectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

        Products = databaseWithCredentials.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));

        // Seed catalog data
        CatalogContextSeed.SeedData(Products);
    }

    public IMongoCollection<Product> Products { get; }
}
