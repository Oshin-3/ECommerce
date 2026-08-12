using ECommerce.Application.DTOs.Common;
using ECommerce.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CheckOutAsync(Guid userId);
        Task<List<OrderResponseDto>> GetMyOrdersAsync(Guid userId, PaginationQueryDto paginationQuery);
        Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId, Guid userId);
        Task<OrderResponseDto> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequestDto request);
        Task<bool> CancelMyOrderAsync(Guid orderId, Guid userId);
        Task<List<OrderResponseDto>> GetAllOrdersAsync(PaginationQueryDto paginationQuery);
    }
}
