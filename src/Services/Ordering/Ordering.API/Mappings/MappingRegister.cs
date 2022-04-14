using EventBus.Messages.Events;
using Mapster;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering.API.Mappings;
public class MappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Map OrderDto
        config.NewConfig<CheckoutOrderCommand, BasketCheckoutEvent>()
            .GenerateMapper(MapType.Map | MapType.MapToTarget | MapType.Projection);
    }
}
