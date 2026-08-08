using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        #region Add To Cart
        [HttpPost]
        [Route("items/add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto addToCartRequestDto)
        {
            //get the authorize userId
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdValue))
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdValue);
            //add to the cart
            await _cartService.AddToCartAsync(userId, addToCartRequestDto);

            return Ok(new
            {
                Message = "Product added to the cart successfully!"
            });
        }

        #endregion

        #region Get the Cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdValue))
            {
                return Unauthorized();
            }
            var userId = Guid.Parse(userIdValue);

            var cart = await _cartService.GetCartAsync(userId);

            return Ok(cart);
        }

        #endregion

        #region Update the Cart
        [HttpPut]
        [Route("items/{cartItemId}")]
        public async Task<IActionResult> UpdateCart(Guid cartItemId, [FromBody] UpdateCartRequestDto updateCartRequestDto)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userIdValue == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdValue);
            var response = await _cartService.UpdateCartAsync(userId, cartItemId, updateCartRequestDto);

            if(!response)
            {
                return NotFound("Cart Item not found");
            }

            return Ok(new {
                Message = "Updated successfully."
            });
        }

        #endregion

        #region Delete the CartItem
        [HttpDelete]
        [Route("items/remove/{cartItem}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartItem)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdValue);

            //remove the cart 
            var response = await _cartService.DeleteCartItemAsync(userId, cartItem);
            if (!response)
            {
                return NotFound("Cart Item not found");
            }

            return Ok(new
            {
                Message = "Remove cart"
            });

        }
        #endregion
    }
}
