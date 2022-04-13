using MapsterMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using Serilog;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList;

public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private IMapper _mapper = new Mapper();   

    public GetOrdersListQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<List<OrderDto>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersByUsernameAsync(request.UserName!, cancellationToken);
        var ordersDto = _mapper.Map<List<OrderDto>>(orders);

        Log.Information("{OrderNumbers} order(s) fetched for {UserName}", orders.Count(), request.UserName);
        return ordersDto;
    }
}