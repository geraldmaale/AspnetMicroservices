using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Respositories;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(OrderDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByUsernameAsync(string userName, CancellationToken cancellationToken)
    {
        var ordersByUser = await DbContext.Orders!
            .Where(o => o.UserName == userName)
            .ToListAsync(cancellationToken);

        return ordersByUser;
    }
}