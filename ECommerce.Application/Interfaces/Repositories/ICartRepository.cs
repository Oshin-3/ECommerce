using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(Guid userId);
        Task<Cart> CreateCartAsync(Cart cart);
        Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task SaveChangesAsync();

    }
}
