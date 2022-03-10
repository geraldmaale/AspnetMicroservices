using Mapster;
using Ordering.Domain.Entities;

namespace Ordering.Application.Mappings;
public class MappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Map Coupon
        config.NewConfig<Order, Order>()
            .GenerateMapper(MapType.Map | MapType.MapToTarget | MapType.Projection);
    }
}
