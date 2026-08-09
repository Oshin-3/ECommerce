using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ECommerceDbContext _dbContext;
        public OrderRepository(ECommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddOrderAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
        }

        public async Task AddOrderItemAsync(IEnumerable<OrderItem> orderItems)
        {
            await _dbContext.OrderItems.AddRangeAsync(orderItems);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
