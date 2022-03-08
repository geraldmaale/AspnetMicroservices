using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ICatalogContext _context;

    public ProductRepository(ICatalogContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task Create(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        await _context.Products.InsertOneAsync(product);
    }

    public async Task<bool> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);

        DeleteResult deleteResult = await _context
            .Products
            .DeleteOneAsync(filter);

        return deleteResult.IsAcknowledged
            && deleteResult.DeletedCount > 0;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var products = await _context.Products.Find(p => true).ToListAsync();
        return products;
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string name)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, name);

        var products = await _context.Products.Find(filter).ToListAsync();
        return products;
    }

    public async Task<Product> GetByIdAsync(string id)
    {
        var product = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        return product;
    }

    public async Task<IEnumerable<Product>> GetByNameAsync(string name)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);

        var products = await _context.Products.Find(filter).ToListAsync();
        return products;
    }

    public async Task<bool> Update(Product product)
    {
        var result = await _context.Products.ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
