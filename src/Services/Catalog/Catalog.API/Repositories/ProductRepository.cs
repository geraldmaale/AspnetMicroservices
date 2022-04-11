using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{

    public ProductRepository(ICatalogContext context) : base(context)
    {
    }

    public override string? CollectionName { get; set; } = CatalogContext.ProductCollection;


    public async Task<IEnumerable<Product>> GetByCategoryAsync(string name)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category!.Name, name);

        var products = await MongoCollection.Find(filter).ToListAsync();
        return products;
    }

    public async Task<IEnumerable<Product>> GetByNameAsync(string name)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);

        var products = await MongoCollection.Find(filter).ToListAsync();
        return products;
    }


}
