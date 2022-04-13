using MediatR;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder;

public record CheckoutOrderCommand
(
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
) : IRequest<Guid>;