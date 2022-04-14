using System.Net.Mime;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using GreatIdeas.Extensions;
using MapsterMapper;
using MassTransit;
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
    private readonly DiscountGrpcService _discountGrpcService;
    private readonly IPublishEndpoint _publishEndpoint;
    private IMapper _mapper = new Mapper();

    public BasketController(IBasketRepository basketRepository,
        ILogger<BasketController> logger, DiscountGrpcService discountGrpcService, IPublishEndpoint publishEndpoint)
    {
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult<ShoppingCart>))]
    public async Task<IActionResult> GetBasket(string userName)
    {
        try
        {
            var shoppingCart = await _basketRepository.GetBasketAsync(userName);
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
            _logger.LogError(ex, StaticExceptions.GetException);
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
            // Communicate with Discount.Grpc
            // and Calculate the latest prices of products into the shopping cart
            foreach (var item in shoppingCart.Items)
            {
                var coupon = await _discountGrpcService.GetDiscountAsync(item.ProductName!);

                item.Price -= (decimal)coupon.Amount;
            }

            var result = await _basketRepository.UpdateAsync(shoppingCart);

            _logger.LogInformation("Basket with username: {UserName} updated successfully", shoppingCart.UserName);
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, StaticExceptions.UpdateException);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to update record with username: {shoppingCart.UserName}" });
        }
    }

    [HttpDelete("{username}", Name = "DeleteBasket")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> DeleteBasket(string username)
    {
        try
        {
            await _basketRepository.DeleteAsync(username);

            _logger.LogInformation("Basket with username: {UserName} deleted successfully", username);
            return Ok(new ApiResult() { IsSuccessful = true, Message = "Deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, StaticExceptions.DeleteException);
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to delete results with username: {username}" });
        }
    }


    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResult))]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {

        try
        {
            // Get existing basket with total price
            var basket = await _basketRepository.GetBasketAsync(basketCheckout.UserName);
            if (basket is null)
            {
                _logger.LogError("Basket with username: {UserName} not found", basketCheckout.UserName);
                return NotFound(new ApiResult { Message = $"Basket with username: {basketCheckout.UserName} not found" });
            }

            // Send checkout event to RabbitMQ
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);

            // Remove the basket
            await _basketRepository.DeleteAsync(basket.UserName!);

            _logger.LogInformation("Basket with username: {UserName} checked out successfully", basketCheckout.UserName);
            return Accepted();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, string.Format("Failed to checkout basket with username: {UserName}", basketCheckout.UserName));
            return UnprocessableEntity(new ApiResult() { Message = $"Failed to checkout basket with username: {basketCheckout.UserName}" });
        }

    }
}