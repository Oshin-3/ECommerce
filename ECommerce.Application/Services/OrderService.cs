using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
       
        public OrderService(ICartRepository cartRepository, IOrderRepository orderRepository)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
        }
        public async Task<OrderResponseDto> CheckOutAsync(Guid userId)
        {
            //fetch user's cart
            var userCart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (userCart == null)
            {
                throw new InvalidOperationException("Cart not found.");
            }
            if(userCart.CartItems == null || !userCart.CartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty.");
            }

            //vaidate stock
            foreach (var cartItem in userCart.CartItems)
            {
                if(cartItem.Product == null)
                {
                    throw new InvalidOperationException("Product information could not be loaded.");
                }

                if(cartItem.Quantity > cartItem.Product.StockQuantity)
                {
                    throw new InvalidOperationException($"Insuffient stock for the product {cartItem.Product.Name}.");
                }
            }

            //total cart amount
            var totalAmount = userCart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            //create order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending.ToString()

            };

            await _orderRepository.AddOrderAsync(order);
            

            //create orderItem
            var orderItems = userCart.CartItems.Select(cartItem => new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Product.Price
            }).ToList();

            await _orderRepository.AddOrderItemAsync(orderItems);
            
            //remove the stock quantity of the product
            foreach (var cartItem in userCart.CartItems)
            {
                cartItem.Product.StockQuantity -= cartItem.Quantity;
            }            

            //clear cart
            _cartRepository.DeleteCartItemAsync(userCart.CartItems);

            await _orderRepository.SaveChangesAsync();
            var response = new OrderResponseDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending.ToString(),
                Items = orderItems.Select(orderId => new OrderItemResponseDto
                {
                    OrderItemId = orderId.Id,
                    ProductId = orderId.ProductId,
                    ProductName = orderId.Product.Name,
                    Quantity = orderId.Quantity,
                    UnitPrice = orderId.UnitPrice
                }).ToList()
            };

            return response;
        }

        public async Task<List<OrderResponseDto>> GetMyOrdersAsync(Guid userId)
        {
            var orders = await _orderRepository.GetOrderByUserIdAsync(userId);
            if(orders == null)
            {
                throw new InvalidOperationException("No orders were found!");
            }
            var response = new List<OrderResponseDto>();
            foreach(var order in orders)
            {
                response.Add(new OrderResponseDto
                {
                    OrderId = order.Id,
                    Items = order.OrderItems.Select(orderItem => new OrderItemResponseDto
                    {
                        OrderItemId = orderItem.Id,
                        ProductId = orderItem.ProductId,
                        ProductName = orderItem.Product.Name,
                        UnitPrice = orderItem.UnitPrice,
                        Quantity = orderItem.Quantity
                    }).ToList(),
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount
                });
            }

            return response;
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId, userId);
            if (order == null)
            {
                throw new Exception("No order found");
            }

            var response = new OrderResponseDto
            {
                OrderId = order.Id,
                Items = order.OrderItems.Select(orderItem => new OrderItemResponseDto
                {
                    OrderItemId = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product.Name,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice
                }).ToList(),
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount
            };

            return response;
        }
    }
}
