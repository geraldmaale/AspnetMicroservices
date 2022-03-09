using System.Net.Mime;
using Discount.Shared.Entities;
using Discount.Shared.Repositories;
using GreatIdeas.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ApiResult))]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ILogger<DiscountsController> _logger;

    public DiscountsController(IDiscountRepository discountRepository, ILogger<DiscountsController> logger)
    {
        _discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    [HttpGet("{productName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult<Coupon>))]
    public async Task<IActionResult> GetDiscount(string productName)
    {
        try
        {
            var coupon = await _discountRepository.GetFirstOrDefaultAsync(p => p.ProductName == productName);
            if (coupon == null)
            {
                return NotFound(new ApiResult() {
                    Message = $"Discount for {productName} not found"
                });
            }

            return Ok(new ApiResult<Coupon>() {
                IsSuccessful = true,
                Result = coupon,
                Message = "Success"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get coupon: {ex.Message}");
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to get coupon for {productName}" });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> CreateDiscount([FromBody] Coupon coupon)
    {
        try
        {
            var result = await _discountRepository.InsertAsync(coupon);
            await _discountRepository.SaveChangesAsync();

            _logger.LogInformation($"Discount for {coupon.ProductName} created successfully");
            return Ok(new ApiResult<Coupon>() {
                IsSuccessful = true,
                Result = coupon,
                Message = "Created successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to update coupon: {ex.Message}");
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to create coupon for {coupon.ProductName}" });
        }
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> UpdateDiscount([FromBody] Coupon coupon)
    {
        try
        {
            _discountRepository.Update(coupon);
            await _discountRepository.SaveChangesAsync();

            _logger.LogInformation($"Discount for {coupon.ProductName} updated successfully");
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to update discount: {ex.Message}");
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to update coupon for {coupon.ProductName}" });
        }
    }

    [HttpDelete("{productName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> DeleteDiscount(string productName)
    {
        try
        {
            var coupon = await _discountRepository.GetFirstOrDefaultAsync(p => p.ProductName == productName);

            if (coupon == null)
            {
                _logger.LogError($"Coupon for {productName} could not be found");
                return Ok(new ApiResult() {
                    IsSuccessful = true,
                    Message = $"Coupon for {productName} could not be found"
                });
            }

            _discountRepository.Delete(coupon);
            await _discountRepository.SaveChangesAsync();

            _logger.LogInformation($"Discount for {productName} deleted successfully");
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete coupon: {ex.Message}");
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to delete coupon for {productName}" });
        }
    }

}