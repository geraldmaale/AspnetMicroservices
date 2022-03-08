using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;
    private const string? _productCollection = "Products";

    public ProductRepository(ICatalogContext context)
    {
        _products = context.ConnectToMongo<Product>(_productCollection);
    }

    public async Task Create(Product product)
    {
        await _products.InsertOneAsync(product);
    }

    public async Task<bool> Delete(string id)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);

        DeleteResult deleteResult = await _products
            .DeleteOneAsync(filter);

        return deleteResult.IsAcknowledged
            && deleteResult.DeletedCount > 0;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var products = await _products.Find(_ => true).ToListAsync();
        return products;
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string name)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, name);

        var products = await _products.Find(filter).ToListAsync();
        return products;
    }

    public async Task<Product> GetByIdAsync(string id)
    {
        var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
        return product;
    }

    public async Task<IEnumerable<Product>> GetByNameAsync(string name)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);

        var products = await _products.Find(filter).ToListAsync();
        return products;
    }

    public async Task<bool> Update(Product product)
    {
        var result = await _products.ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
