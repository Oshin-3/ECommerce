using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.DTOs.CartItem;
using ECommerce.Application.Exceptions;
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
                throw new BusinessRuleException("Quantity must be greater than zero.");
            }
            //check if product exists
            var product = await _productRepository.GetProductByIdAsync(request.ProductId);
            if (product == null)
            {
                throw new NotFoundException("No Product found!");
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
                
            }
            else
            {
                //increae the quantity
                cartItem.Quantity += request.Quantity;
            }

            await _cartRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteCartItemAsync(Guid userId, Guid cartItemId)
        {
            //check if cartItem exists
            var cartItem = await _cartRepository.GetCartItemByIdAsync(userId, cartItemId);
            if (cartItem == null)
                return false;

            
            //delete
            _cartRepository.DeleteCartItemAsync((IEnumerable<CartItem>)cartItem);
            await _cartRepository.SaveChangesAsync();
            return true;
        }

        public async Task<GetCartResponseDto> GetCartAsync(Guid userId)
        {
            //fetch the cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return null; 
            }

            //Convert cartItem to response dto
            var items = cart.CartItems.Select(cartItem => new CartItemResponseDto
            {
                CartItemId = cartItem.Id,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.Name,
                Price = cartItem.Product.Price,
                Quantity = cartItem.Quantity,
                SubTotal = cartItem.Product.Price * cartItem.Quantity
            }).ToList();

            //create final cart response 
            var response = new GetCartResponseDto
            {
                CartId = cart.Id,
                Items = items,
                TotolAmount = items.Sum(item => item.SubTotal)
           
            };

            return response;
           
        }

        public async Task<bool> UpdateCartAsync(Guid userId, Guid cartItemId, UpdateCartRequestDto request)
        {
            //verify quantity
            if(request.Quantity <= 0)
            {
                throw new BusinessRuleException("Quantity must be greater than zero.");
            }

            var cartItem = await _cartRepository.GetCartItemByIdAsync(userId, cartItemId);
            if (cartItem == null)
                return false;

            cartItem.Quantity = request.Quantity;

            await _cartRepository.SaveChangesAsync();

            return true;


        }
    }
}
