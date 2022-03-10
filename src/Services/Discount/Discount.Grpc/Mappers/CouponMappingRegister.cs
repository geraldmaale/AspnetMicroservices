using Discount.Grpc.Protos;
using Discount.Shared.Entities;
using Mapster;

namespace Discount.Grpc.Mappers;
public class CouponMappingRegister: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Map Coupon
        config.NewConfig<Coupon, CouponModel>()
            .GenerateMapper(MapType.Map | MapType.MapToTarget | MapType.Projection);
    }
}
