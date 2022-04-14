using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<CheckoutOrderCommandHandler> _logger;
    private IMapper _mapper = new Mapper();

    public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IEmailService emailService,
        ILogger<CheckoutOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger;
    }

    public async Task<Guid> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var orderEntity = _mapper.Map<Order>(request);
            var createdOrder = await _orderRepository.AddAsync(orderEntity, cancellationToken);

            _logger.LogInformation("Order with id {OrderId} has been successfully created", createdOrder.Id);

            // Send email to customer
            await SendEmailAsync(orderEntity);

            return createdOrder.Id;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw new Exception($"Checkout order failed for {request.UserName}");
        }
    }

    private async Task SendEmailAsync(Order order)
    {
        try
        {
            // Send order email
            var email = new Email() {
                To = order.EmailAddress,
                Subject = "Order confirmation",
                Body = $"Your order with Order Id: {order.Id} has been successfully created.",
                From = "Order System"
            };

            await _emailService.SendEmail(email);

            _logger.LogInformation("Order email has been sent to {UserEmail}", order.EmailAddress);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Order {OrderId} email sending failed for {UserName}", order.Id, order.UserName);
        }
    }
}