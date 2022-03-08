using System.Net.Mime;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
        {
            _productRepository = productRepository ?? throw new System.ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get products: {ex.Message}");
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpGet("{productId:length(24)}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        public async Task<IActionResult> GetProduct(string productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    _logger.LogError($"Product with id {productId} not found");
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get product: {ex.Message}");
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpGet("[action]/{name}", Name = "GetProductsByName")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        public async Task<IActionResult> GetProductsByName(string name)
        {
            try
            {
                var products = await _productRepository.GetByNameAsync(name);
                if (!products.Any())
                {
                    _logger.LogError($"Product with name {name} not found");
                    return NotFound();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get product: {ex.Message}");
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpGet("[action]/{category}", Name = "GetProductsByCategory")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            try
            {
                var products = await _productRepository.GetByCategoryAsync(category);
                if (!products.Any())
                {
                    _logger.LogError($"Products with category {category} not found");
                    return NotFound();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get products: {ex.Message}");
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            try
            {
                await _productRepository.Create(product);
                return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create product: {ex.Message}");
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPut("{productId:length(24)}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(string productId, [FromBody] Product product)
        {
            try
            {
                var productToUpdate = await _productRepository.GetByIdAsync(productId);
                if (productToUpdate == null)
                {
                    _logger.LogError($"Product with id {productId} not found");
                    return NotFound();
                }
                var result = await _productRepository.Update(product);
                if (!result)
                {
                    _logger.LogError($"Failed to update product with id {productId}");
                    return UnprocessableEntity();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update product: {ex.Message}");
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpDelete("{productId:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            try
            {
                var productToDelete = await _productRepository.GetByIdAsync(productId);
                if (productToDelete == null)
                {
                    _logger.LogError($"Product with id {productId} not found");
                    return NotFound();
                }
                var result = await _productRepository.Delete(productId);
                if (!result)
                {
                    _logger.LogError($"Failed to delete product with id {productId}");
                    return UnprocessableEntity();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete product: {ex.Message}");
                return UnprocessableEntity(ex.Message);
            }
        }

    }
}
