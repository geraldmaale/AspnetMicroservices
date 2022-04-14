using Basket.API.Entities;
using EventBus.Messages.Events;
using Mapster;

namespace Basket.API.Mappers;

public class MappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Map BasketCheckout to BasketCheckoutEvent
        config.NewConfig<BasketCheckout, BasketCheckoutEvent>()
            .GenerateMapper(MapType.Map | MapType.MapToTarget | MapType.Projection);
    }
}