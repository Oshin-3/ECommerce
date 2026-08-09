using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task AddOrderItemAsync(IEnumerable<OrderItem> orderItems);
        Task SaveChangesAsync();

    }
}
