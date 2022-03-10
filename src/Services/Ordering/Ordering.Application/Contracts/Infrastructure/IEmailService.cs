namespace Ordering.Application.Contracts.Infrastructure;
public interface IEmailService
{
    Task<bool> SendEmailAsync(string email);
}
