using ECommerce.Application.DTOs;
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
        Task<List<Order?>> GetOrderByUserIdAsync(Guid userId, PaginationQueryDto paginationQuery);
        Task<Order> GetOrderByIdAsync(Guid orderId, Guid userId);
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<List<Order>> GetAllOrdersAsync(PaginationQueryDto paginationQuery);
    }
}
