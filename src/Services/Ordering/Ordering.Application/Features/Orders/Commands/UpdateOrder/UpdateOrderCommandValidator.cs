using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(co => co.UserName)
            .NotEmpty()
            .WithMessage("Username cannot be empty")
            .MaximumLength(50)
            .WithMessage("Username cannot exceed 50 characters");

        RuleFor(co => co.FirstName)
            .NotEmpty()
            .WithMessage("First Name is required")
            .MaximumLength(15)
            .WithMessage("First Name cannot exceed 15 characters");

        RuleFor(co => co.LastName)
            .NotEmpty()
            .WithMessage("Last Name is required")
            .MaximumLength(15)
            .WithMessage("Last Name cannot exceed 15 characters");

        RuleFor(co => co.EmailAddress)
            .NotEmpty()
            .WithMessage("Email Address is required")
            .EmailAddress()
            .WithMessage("Email address is not valid");

        RuleFor(co => co.Country)
            .NotEmpty()
            .WithMessage("Country is required");

        RuleFor(co => co.State)
            .NotEmpty()
            .WithMessage("State is required");

        RuleFor(co => co.ZipCode)
            .NotEmpty()
            .WithMessage("ZipCode is required");

        RuleFor(co => co.AddressLine)
            .NotEmpty()
            .WithMessage("AddressLine1 is required");

        RuleFor(co => co.CVV)
            .NotEmpty()
            .WithMessage("CVV is required")
            .Length(3, 4)
            .WithMessage("CVV must be 3 or 4 characters");

        RuleFor(co => co.CardNumber)
            .NotEmpty()
            .WithMessage("CardNumber is required")
            .Length(12, 19)
            .WithMessage("CardNumber must be between 12 and 19 characters");

        RuleFor(co => co.CardName)
            .NotEmpty()
            .WithMessage("CardName is required")
            .Length(5, 50)
            .WithMessage("CardName must be between 5 and 50 characters");
        RuleFor(co => co.Expiration)
            .NotEmpty()
            .WithMessage("Expiration is required")
            .Length(5)
            .WithMessage("Expiration must be 5 characters");

        RuleFor(co => co.TotalPrice)
            .NotEmpty()
            .WithMessage("Total Price is required")
            .GreaterThan(0)
            .WithMessage("Total Price must be greater than 0");
    }
}