using System.Net.Mime;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using GreatIdeas.Extensions;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Catalog.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ApiResult))]
// [ResponseCache(CacheProfileName = Constants.FiveMinutesCacheProfileResponse)]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResults<Product>))]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = Constants.OneMinutesCacheResponse)]
    // [HttpCacheValidation(MustRevalidate = false)]
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(new ApiResults<Product>() { IsSuccessful = true, Message = "Success", Results = products });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get products: {Message}", ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to get products" });
        }
    }

    [HttpGet("{productId:length(24)}", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResults<Product>))]
    public async Task<IActionResult> GetProduct(string productId)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(p => p.Id == productId);
            if (product is null)
            {
                _logger.LogError("Product with id {ProductId} not found", productId);
                return NotFound(new ApiResult() { Message = $"Product with id {productId} not found" });
            }
            return Ok(new ApiResult<Product>() { IsSuccessful = true, Result = product, Message = "Success" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get product: {Message}", ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to get products: {productId}" });
        }
    }

    [HttpGet("[action]/{name}", Name = "GetProductsByName")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult<Product>))]
    public async Task<IActionResult> GetProductsByName(string name)
    {
        try
        {
            var products = await _productRepository.GetByNameAsync(name);
            var enumerable = products.ToList();
            if (!enumerable.Any())
            {
                _logger.LogError("Product with name {ProductName} not found", name);
                return NotFound(new ApiResult() { Message = $"Product with name {name} not found" });
            }

            return Ok(new ApiResults<Product>() { IsSuccessful = true, Results = enumerable, Message = "Success" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get product {ProductName}: {Message}", name, ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to get products: {name}" });
        }
    }

    [HttpGet("[action]/{category}", Name = "GetProductsByCategory")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResults<Product>))]
    public async Task<IActionResult> GetProductsByCategory(string category)
    {
        try
        {
            var products = await _productRepository.GetByCategoryAsync(category);
            if (!products.Any())
            {
                _logger.LogError("Product with category {Category} not found", category);
                return NotFound(new ApiResult() { Message = $"Product with category {category} not found" });
            }
            return Ok(new ApiResults<Product>() { IsSuccessful = true, Results = products, Message = "Success" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get products by category: {Message}", ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to get products by category: {category}" });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResult))]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        try
        {
            await _productRepository.CreateAsync(product);
            _logger.LogInformation("Product: {ProductName} created successfully", product.Name);
            return CreatedAtRoute(nameof(GetProduct), new { productId = product.Id }, product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create product: {Message}", ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to create product" });
        }
    }

    [HttpPut("{productId:length(24)}", Name = "UpdateProduct")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> UpdateProduct(string productId, [FromBody] Product product)
    {
        try
        {
            var result = await _productRepository.UpdateAsync(product, FilterId(productId));
            if (!result)
            {
                _logger.LogError("Failed to update product with id {ProductId}", productId);
                return BadRequest(new ApiResult() { Message = $"Failed to update product with id: {productId}" });
            }

            _logger.LogInformation("Product: {ProductId} updated successfully", productId);
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update product: {Message}", ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to update products: {productId}" });
        }
    }

    [HttpDelete("{productId:length(24)}", Name = "DeleteProduct")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> DeleteProduct(string productId)
    {
        try
        {
            var result = await _productRepository.DeleteAsync(FilterId(productId));
            if (!result)
            {
                _logger.LogError("Failed to delete product with id {ProductId}", productId);
                return BadRequest(new ApiResult() { Message = $"Failed to delete product with id {productId}" });
            }

            _logger.LogInformation("Product: {ProductId} deleted successfully", productId);
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete product: {Message}", ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to delete products: {productId}" });
        }
    }

    private FilterDefinition<Product> FilterId(string? productId)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, productId);
        return filter;
    }

}