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
        [Route("add/items")]
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

        #region

        #endregion
    }
}
