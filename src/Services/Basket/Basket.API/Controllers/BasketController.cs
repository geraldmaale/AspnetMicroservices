using System.Net.Mime;
using Basket.API.Entities;
using Basket.API.Repositories;
using GreatIdeas.Extensions;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ApiResult))]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger<BasketController> _logger;
    private IMapper _mapper = new Mapper();

    public BasketController(IBasketRepository basketRepository, ILogger<BasketController> logger)
    {
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult<ShoppingCart>))]
    public async Task<IActionResult> GetBasket(string userName)
    {
        try
        {
            var shoppingCart = await _basketRepository.GetAsync(userName);
            if (shoppingCart == null)
            {
                var newCart = new ShoppingCart(userName);
                return Ok(new ApiResult<ShoppingCart>() {
                    Result = new ShoppingCart(userName),
                    IsSuccessful = true,
                    Message = "Created new shopping cart info"
                });
            }

            return Ok(new ApiResult<ShoppingCart>() {
                IsSuccessful = true,
                Result = shoppingCart,
                Message = "Success"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get basket: {ex.Message}");
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to get shopping cart with username {userName}" });
        }
    }

    [HttpPost(Name = "UpdateBasket")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> UpdateBasket([FromBody] ShoppingCart shoppingCart)
    {
        try
        {
            var result = await _basketRepository.UpdateAsync(shoppingCart);

            _logger.LogInformation($"Basket with username: {shoppingCart.UserName} updated successfully");
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to update basket: {ex.Message}");
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to update results with username: {shoppingCart.UserName}" });
        }
    }

    [HttpDelete("{username}", Name = "DeleteBasket")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> DeleteBasket(string username)
    {
        try
        {
            await _basketRepository.DeleteAsync(username);

            _logger.LogInformation($"Basket with username: {username} deleted successfully");
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete basket: {ex.Message}");
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to delete results with username: {username}" });
        }
    }

}