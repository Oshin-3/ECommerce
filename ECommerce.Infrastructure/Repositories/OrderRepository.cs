using ECommerce.Application.DTOs.Common;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enum;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Order>> GetAllOrdersAsync(PaginationQueryDto paginationQuery)
        {
            var query = _dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            var skip = (paginationQuery.PageNumber - 1) * paginationQuery.PageSize;
            var take = paginationQuery.PageSize;
            var response = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            if (response == null)
                return null;
            return response;
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId, Guid userId)
        {
            var resposne = await _dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            if (resposne == null)
                return null;
            return resposne;
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            var response = await _dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (response == null)
                return null;
            return response;
        }

        public async Task<List<Order?>> GetOrderByUserIdAsync(Guid userId, PaginationQueryDto paginationQuery)
        {
            var skip = (paginationQuery.PageNumber - 1) * paginationQuery.PageSize;
            var take = paginationQuery.PageSize;
            var response = await _dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Skip(skip)
                .Take(take)
                .Where(o => o.UserId == userId).ToListAsync();
            if (response == null)
                return null;
            return response;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        
    }
}
