using ECommerce.Application.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(Guid userId, AddToCartRequestDto request);
        Task<GetCartResponseDto> GetCartAsync(Guid userId);
        Task<bool> UpdateCartAsync(Guid userId, Guid cartItemId, UpdateCartRequestDto request);
        Task<bool> DeleteCartItemAsync(Guid userId, Guid cartItemId);
    }
}
