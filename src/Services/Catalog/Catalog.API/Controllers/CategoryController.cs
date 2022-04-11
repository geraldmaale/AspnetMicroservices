using System.Net.Mime;
using Catalog.API.DTOs;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using GreatIdeas.Extensions;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Serilog;

namespace Catalog.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ApiResult))]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryController> _logger;
    private IMapper _mapper = new Mapper();

    public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResults<CategoryDto>))]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var results = await _categoryRepository.GetAllAsync();
            var categories = _mapper.Map<List<CategoryDto>>(results);
            _logger.Log(LogLevel.Information, "{CategoriesCount} categories found", categories.Count);
            
            return Ok(new ApiResults<CategoryDto>() {
                IsSuccessful = true,
                Message = "Success",
                Results = categories
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get results: {Message}", ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to get results" });
        }
    }

    [HttpGet("{categoryId:length(24)}", Name = "GetCategory")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResults<CategoryDto>))]
    public async Task<IActionResult> GetCategory(string categoryId)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(c => c.Id == categoryId);
            if (category == null)
            {
                _logger.LogError("Category with id {CategoryId} not found", categoryId);
                return NotFound(new ApiResult() { Message = $"Category was not found" });
            }

            _logger.LogInformation("{Category} Categories fetch successfully", category.Id);
            return Ok(new ApiResult<CategoryDto>() {
                IsSuccessful = true,
                Result = _mapper.Map<CategoryDto>(category),
                Message = "Success"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get category: {Message}", ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to get category: {categoryId}" });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResult))]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreationDto category)
    {
        try
        {
            var entity = _mapper.Map<Category>(category);
            await _categoryRepository.CreateAsync(entity);

            _logger.LogInformation("Category: {CategoryName} created successfully", category.Name);
            return CreatedAtRoute(nameof(GetCategory), new { categoryId = entity.Id }, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create category: {Message}", ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to create category" });
        }
    }

    [HttpPut(Name = "UpdateCategory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDto category)
    {
        try
        {
            var entityToUpdate = _mapper.Map<Category>(category);
            var result = await _categoryRepository.UpdateAsync(entityToUpdate, FilterId(entityToUpdate.Id));
            if (!result)
            {
                _logger.LogError("Failed to update category with id {CategoryId}", entityToUpdate.Id);
                return BadRequest(new ApiResult() { Message = $"Failed to update category with id: {entityToUpdate.Id}" });
            }

            _logger.LogInformation("Category: {CategoryId} updated successfully", category.Id);
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update category: {Message}", ex.Message);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to update category: {category.Id}" });
        }
    }

    [HttpDelete("{categoryId:length(24)}", Name = "DeleteCategory")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> DeleteCategory(string categoryId)
    {
        try
        {
            var result = await _categoryRepository.DeleteAsync(FilterId(categoryId));
            if (!result)
            {
                _logger.LogError("Failed to delete category with id {CategoryId}", categoryId);
                return BadRequest(new ApiResult() { Message = $"Failed to delete category with id {categoryId}" });
            }

            _logger.LogInformation("Category: {CategoryId} deleted successfully", categoryId);
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete category: {Message}", categoryId);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to delete results: {categoryId}" });
        }
    }

    private FilterDefinition<Category> FilterId(string? categoryId)
    {
        FilterDefinition<Category> filter = Builders<Category>.Filter.Eq(p => p.Id, categoryId);
        return filter;
    }
}