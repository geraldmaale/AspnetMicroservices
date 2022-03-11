using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder;

public class DeleteOrderCommandHandler: IRequestHandler<DeleteOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<DeleteOrderCommandHandler> _logger;
    private IMapper _mapper = new Mapper();

    public DeleteOrderCommandHandler(IOrderRepository orderRepository, IEmailService emailService, 
        ILogger<DeleteOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger;
    }
    
    public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);
            if (orderToUpdate == null)
            {
                _logger.LogError($"Order with id: {request.Id} was not found.");
                throw new NotFoundException(nameof(Order), request.Id);
            }
            
            await _orderRepository.DeleteAsync(orderToUpdate);
        
            _logger.LogInformation($"Order with id {request.Id} has been successfully deleted.");
            
            return Unit.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw new Exception($"Checkout order update failed for {request.Id}");
        }
    }
}