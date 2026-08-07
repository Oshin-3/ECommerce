using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        public CartService(ICartRepository cartRespository, IProductRepository productRepository)
        {
            _cartRepository = cartRespository;
            _productRepository = productRepository;
        }
        public async Task AddToCartAsync(Guid userId, AddToCartRequestDto request)
        {
            //if quantity is 0
            if(request.Quantity <= 0)
            {
                throw new Exception("Quantity must be greater than zero.");
            }
            //check if product exists
            var product = await _productRepository.GetProductByIdAsync(request.ProductId);
            if (product == null)
            {
                throw new Exception("No Product found!");
            }               

            //get the user cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                //create cart
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId
                };

                await _cartRepository.CreateCartAsync(cart);
            }

            //get the cartItem 
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, request.ProductId);
            if (cartItem == null)
            {
                //add new item in cartItem
                cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                await _cartRepository.AddCartItemAsync(cartItem);
                await _cartRepository.SaveChangesAsync();
            }
            else
            {
                //increae the quantity
                cartItem.Quantity += request.Quantity;
            }

            await _cartRepository.SaveChangesAsync();
        }
    }
}
