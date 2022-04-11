using Ordering.Application.Models;

namespace Ordering.Application.Contracts.Infrastructure;
public interface IEmailService
{
    Task<bool> UseMsGraph(Email email);
}
