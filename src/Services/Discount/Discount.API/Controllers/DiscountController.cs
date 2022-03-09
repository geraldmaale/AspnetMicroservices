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
public class DiscountController : ControllerBase
{
    private readonly IDiscountDapperRepository _discountDapperRepository;
    private readonly ILogger<DiscountController> _logger;

    public DiscountController(IDiscountDapperRepository discountDapperRepository, ILogger<DiscountController> logger)
    {
        _discountDapperRepository = discountDapperRepository ?? throw new ArgumentNullException(nameof(discountDapperRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    [HttpGet("{productName}", Name = "GetDiscount")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult<Coupon>))]
    public async Task<IActionResult> GetDiscount(string productName)
    {
        try
        {
            var coupon = await _discountDapperRepository.GetByProductNameAsync(productName);

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

    [HttpPost(Name = "CreateDiscount")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> CreateDiscount([FromBody] Coupon coupon)
    {
        try
        {
            var result = await _discountDapperRepository.CreateAsync(coupon);

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

    [HttpPut(Name = "UpdateDiscount")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> UpdateDiscount([FromBody] Coupon coupon)
    {
        try
        {
            var result = await _discountDapperRepository.UpdateAsync(coupon);

            _logger.LogInformation($"Discount for {coupon.ProductName} updated successfully");
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to update discount: {ex.Message}");
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to update coupon for {coupon.ProductName}" });
        }
    }

    [HttpDelete("{productName}", Name = "DeleteDiscount")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> DeleteDiscount(string productName)
    {
        try
        {
            var result = await _discountDapperRepository.DeleteAsync(productName);
            if (!result)
            {
                _logger.LogError($"Coupon for {productName} could not be found");
                return Ok(new ApiResult() {
                    IsSuccessful = true,
                    Message = $"Coupon for {productName} could not be found"
                });
            }

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