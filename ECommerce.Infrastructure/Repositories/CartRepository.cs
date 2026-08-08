using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ECommerceDbContext _dbContext;
        public CartRepository(ECommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CartItem> AddCartItemAsync(CartItem cartItem)
        {
            await _dbContext.CartItems.AddAsync(cartItem);
            return cartItem;
        }

        public async Task<Cart> CreateCartAsync(Cart cart)
        {
            await _dbContext.Carts.AddAsync(cart);
            return cart;
        }

        public void DeleteCartItemAsync(CartItem cartItemId)
        {
            _dbContext.CartItems.Remove(cartItemId);
        }

        public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(u => u.UserId == userId);
            if (cart == null)
                return null;
            return cart;
        }

        public async Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId)
        {
            var cartItem = await _dbContext.CartItems.FirstOrDefaultAsync
                (ci => ci.CartId == cartId && ci.ProductId == productId);
            return cartItem;
        }

        public async Task<CartItem?> GetCartItemByIdAsync(Guid userId, Guid cartItemId)
        {
            var cartItem = await _dbContext.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

            return cartItem;
                
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
