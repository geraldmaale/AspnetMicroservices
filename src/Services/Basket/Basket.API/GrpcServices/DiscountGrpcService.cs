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
    
    public async Task<CouponModel> CreateDiscountAsync(CouponModel couponModel)
    {
        var addDiscountRequest = new CreateDiscountRequest(){ Coupon = couponModel };
        var result = await _client.CreateDiscountAsync(addDiscountRequest);
        return result;
    }
    
    public async Task<CouponModel> UpdateDiscountAsync(CouponModel couponModel)
    {
        var updateDiscountRequest = new UpdateDiscountRequest(){ Coupon = couponModel };
        var result = await _client.UpdateDiscountAsync(updateDiscountRequest);
        return result;
    }
    
    public async Task<bool> DeleteDiscountAsync(string productName)
    {
        var deleteDiscountRequest = new DeleteDiscountRequest(){ ProductName = productName };
        var result = await _client.DeleteDiscountAsync(deleteDiscountRequest);
        return result.Success;
    }
}
