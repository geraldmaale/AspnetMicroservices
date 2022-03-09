using Discount.Grpc.Protos;
using Discount.Shared.Entities;
using Discount.Shared.Repositories;
using Grpc.Core;
using MapsterMapper;

namespace Discount.Grpc.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ILogger<DiscountService> _logger;
    private IMapper _mapper = new Mapper();

    public DiscountService(IDiscountRepository discountRepository, ILogger<DiscountService> logger)
    {
        _discountRepository = discountRepository;
        _logger = logger;
    }


    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _discountRepository.GetFirstOrDefaultAsync(c => c.ProductName == request.ProductName);
        if (coupon == null)
        {
            _logger.LogError($"Coupon with product name {request.ProductName} not found");
            throw new RpcException(new Status(StatusCode.NotFound, $"Coupon with product name {request.ProductName} not found"));
        }

        var couponModel = _mapper.Map<CouponModel>(coupon);
        return couponModel;
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = _mapper.Map<Coupon>(request);
        _discountRepository.Insert(coupon);
        await _discountRepository.SaveChangesAsync();

        _logger.LogInformation($"Coupon with product name {coupon.ProductName} created successfully");
        var couponModel = _mapper.Map<CouponModel>(coupon);
        return couponModel;
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _discountRepository.GetFirstOrDefaultAsync(c => c.ProductName == request.Coupon.ProductName);
        if (coupon == null)
        {
            _logger.LogError($"Coupon with product name {request.Coupon.ProductName} not found");
            throw new RpcException(new Status(StatusCode.NotFound, $"Coupon with product name {request.Coupon.ProductName} not found"));
        }

        _mapper.Map(request.Coupon, coupon);

        _discountRepository.Update(coupon);
        await _discountRepository.SaveChangesAsync();

        _logger.LogInformation($"Coupon with product name {coupon.ProductName} updated successfully");
        var couponModel = _mapper.Map<CouponModel>(coupon);
        return couponModel;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _discountRepository.GetFirstOrDefaultAsync(c => c.ProductName == request.ProductName);
        if (coupon == null)
        {
            _logger.LogError($"Coupon with product name {request.ProductName} not found");
            throw new RpcException(new Status(StatusCode.NotFound, $"Coupon with product name {request.ProductName} not found"));
        }

        _discountRepository.Delete(coupon);
        await _discountRepository.SaveChangesAsync();

        _logger.LogInformation($"Coupon with product name {coupon.ProductName} deleted successfully");
        return new DeleteDiscountResponse();
    }
}