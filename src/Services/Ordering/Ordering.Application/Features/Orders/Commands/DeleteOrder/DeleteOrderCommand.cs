using MediatR;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder;

public record DeleteOrderCommand(Guid Id) : IRequest;
//{
//    public Guid Id { get; set; }
//}