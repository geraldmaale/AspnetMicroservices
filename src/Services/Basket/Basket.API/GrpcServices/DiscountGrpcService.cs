using Discount.Grpc.Protos;
using Grpc.Net.Client;

namespace Basket.API.GrpcServices;
public class DiscountGrpcService
{
    private readonly DiscountProtoService.DiscountProtoServiceClient _client;

    public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<CouponModel> GetDiscountAsync(string productName)
    {
        var discountRequest = new GetDiscountRequest() { ProductName = productName };
        var couponModel = await _client.GetDiscountAsync(discountRequest);
        return couponModel;
    }

    public static CouponModel GetCouponAsync(string productName, string grpcUrl)
    {
        using var channel = GrpcChannel.ForAddress(grpcUrl);
        var client = new DiscountProtoService.DiscountProtoServiceClient(channel);

        var discountRequest = new GetDiscountRequest() { ProductName = productName };
        var couponModel = client.GetDiscount(discountRequest);

        return couponModel;
    }
}
