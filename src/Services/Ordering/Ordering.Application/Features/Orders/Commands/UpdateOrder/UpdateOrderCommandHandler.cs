using GreatIdeas.Extensions;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler: IRequestHandler<UpdateOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UpdateOrderCommandHandler> _logger;
    private IMapper _mapper = new Mapper();

    public UpdateOrderCommandHandler(IOrderRepository orderRepository, IEmailService emailService, 
        ILogger<UpdateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger;
    }
    
    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);
            if (orderToUpdate == null)
            {
                _logger.LogError($"Order with id: {request.Id} was not found.");
                throw new NotFoundException(nameof(Order), request.Id);
            }

            _mapper.Map(request, orderToUpdate);
            await _orderRepository.UpdateAsync(orderToUpdate);

            _logger.LogInformation($"Order with id {request.Id} has been successfully updated");

            // Send email to customer
            await SendEmailAsync(orderToUpdate);

            return Unit.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw new Exception($"Checkout order update failed for {request.UserName}");
        }
    }

    private async Task SendEmailAsync(Order order)
    {
        try
        {
            // Send order email
            var email = new Email()
            {
                To = "geraldmaale@hotmail.com",
                Subject = "Order update confirmation",
                Body = $"Your order with Order Id: {order.Id} has been successfully updated.",
                From = "Order System"
            };

            await _emailService.UseSendGrid(email);
            
            _logger.LogInformation($"Order email has been sent to {email.To}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Order {order.Id} email sending failed for {order.UserName}");
        }
    }
}