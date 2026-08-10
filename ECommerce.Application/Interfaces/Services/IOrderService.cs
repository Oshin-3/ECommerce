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
        Task<List<OrderResponseDto>> GetMyOrdersAsync(Guid userId);
        Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId, Guid userId);
    }
}
