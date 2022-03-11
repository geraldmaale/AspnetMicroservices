using Mapster;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;
using Ordering.Domain.Entities;

namespace Ordering.Application.Mappings;
public class MappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Map OrderDto
        config.NewConfig<Order, OrderDto>()
            .GenerateMapper(MapType.Map | MapType.MapToTarget | MapType.Projection);
        
        // Map Order Command
        config.NewConfig<Order, CheckoutOrderCommand>()
            .GenerateMapper(MapType.Map | MapType.MapToTarget | MapType.Projection);
        
        config.NewConfig<Order, UpdateOrderCommand>()
            .GenerateMapper(MapType.Map | MapType.MapToTarget | MapType.Projection);
    }
}
