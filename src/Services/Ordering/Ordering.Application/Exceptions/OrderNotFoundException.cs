namespace Ordering.Application.Exceptions;
public class OrderNotFoundException : NotFoundException
{
    public OrderNotFoundException(Guid orderId)
        : base($"The Order with ID {orderId}' was not found.")
    {
    }
}
