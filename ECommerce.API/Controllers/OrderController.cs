using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #region CheckOut
        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout()
        {
            //get the authorize userId
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdValue))
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdValue);
            var response = await _orderService.CheckOutAsync(userId);

            return Ok(response);
        }
        #endregion

        #region Get My Orders
        [HttpGet]
        [Route("my-order")]
        public async Task<IActionResult> GetMyOrders()
        {
            //get the authorize userId
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdValue))
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdValue);
            var response = await _orderService.GetMyOrdersAsync(userId);
            if (response == null)
            {
                return NotFound("Orders not found!");
            }
            return Ok(response);
        }
        #endregion

        #region Get Order By Id
        [HttpGet]
        [Route("{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            //get the authorize userId
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdValue))
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdValue);
            var response = await _orderService.GetOrderByIdAsync(orderId, userId);
            if (response == null)
            {
                return NotFound("Order not found!");
            }
            return Ok(response);
        }
        #endregion
    }
}
