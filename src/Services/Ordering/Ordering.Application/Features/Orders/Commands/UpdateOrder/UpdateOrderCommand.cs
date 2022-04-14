using EventBus.Messages.Commons;
using MediatR;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder;

public record UpdateOrderCommand
(
    Guid Id,
    string? UserName,
    decimal TotalPrice,
    string? FirstName,
    string? LastName,
    string? EmailAddress,
    string? AddressLine,
    string? Country,
    string? State,
    string? ZipCode,
    string? CardName,
    string? CardNumber,
    string? Expiration,
    string? CVV,
    PaymentMethod PaymentMethod
) : IRequest;