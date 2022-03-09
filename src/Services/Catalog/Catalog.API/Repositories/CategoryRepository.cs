using Catalog.API.Data;
using Catalog.API.Entities;

namespace Catalog.API.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{

    public CategoryRepository(ICatalogContext context) : base(context)
    {
        // CollectionName = CatalogContext.CategoryCollection;
    }

    public override string? CollectionName { get; set; } = CatalogContext.CategoryCollection;
}
