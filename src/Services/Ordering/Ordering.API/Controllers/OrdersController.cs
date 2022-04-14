using System.Net.Mime;
using GreatIdeas.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;

namespace Ordering.API.Controllers;
[Route("api/v1/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ApiResult))]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{username}", Name = "GetOrders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResults<OrderDto>))]
    public async Task<IActionResult> GetOrdersByUsername(string username)
    {
        var query = new GetOrdersListQuery(username);
        var result = await _mediator.Send(query);
        return Ok(new ApiResults<OrderDto>() {
            IsSuccessful = true,
            Message = "Orders retrieved successfully",
            Results = result
        });
    }

    [HttpPost(Name = "CheckoutOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult<Guid>))]
    public async Task<IActionResult> CheckoutOrder([FromBody] CheckoutOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new ApiResult<Guid>() {
            IsSuccessful = true,
            Message = "Order checked out successfully",
            Result = result
        });
    }

    [HttpPut(Name = "UpdateOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderCommand command)
    {
        await _mediator.Send(command);
        return Ok(new ApiResult {
            IsSuccessful = true,
            Message = "Order updated successfully"
        });
    }

    [HttpDelete("{id}", Name = "DeleteOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResult))]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        var command = new DeleteOrderCommand(id);
        await _mediator.Send(command);

        return Ok(new ApiResult {
            IsSuccessful = true,
            Message = "Order deleted successfully"
        });
    }
}
