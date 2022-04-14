using EventBus.Messages.Events;
using MapsterMapper;
using MassTransit;
using MediatR;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Serilog;

namespace Ordering.API.EventBusConsumers;
public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
{
    IMapper _mapper = new Mapper();
    private readonly IMediator _mediator;

    public BasketCheckoutConsumer(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        var command = _mapper.Map<CheckoutOrderCommand>(context.Message);
        var result = await _mediator.Send(command);
        Log.Information("{EventMessage} consumed successfully. Created OrderID: {OrderId}", nameof(BasketCheckoutEvent), result);
    }
}
