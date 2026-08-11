using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
       
        public OrderService(ICartRepository cartRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CancelMyOrderAsync(Guid orderId, Guid userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                //get the order details 
                var order = await _orderRepository.GetOrderByIdAsync(orderId, userId);
                if (order == null)
                {
                    return false;
                }

                //check if status is already cancelled
                if (order.Status == OrderStatus.Cancelled)
                {
                    throw new InvalidOperationException("Order is already Cancelled!");
                }

                if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Confirmed)
                {
                    order.Status = OrderStatus.Cancelled;
                }
                else
                {
                    throw new BusinessRuleException("Cannot cancel the order");
                }
                //retore the stock
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.Product.StockQuantity += orderItem.Quantity;
                }

                await _unitOfWork.CommitTransactionAsync();
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

        }

        public async Task<OrderResponseDto> CheckOutAsync(Guid userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                //fetch user's cart
                var userCart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (userCart == null)
                {
                    throw new NotFoundException("Cart not found.");
                }
                if (userCart.CartItems == null || !userCart.CartItems.Any())
                {
                    throw new InvalidOperationException("Cart is empty.");
                }

                //vaidate stock
                foreach (var cartItem in userCart.CartItems)
                {
                    if (cartItem.Product == null)
                    {
                        throw new NotFoundException("Product information could not be loaded.");
                    }

                    if (cartItem.Quantity > cartItem.Product.StockQuantity)
                    {
                        throw new BusinessRuleException($"Insuffient stock for the product {cartItem.Product.Name}.");
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
                    Status = OrderStatus.Pending

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

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var response = new OrderResponseDto
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    TotalAmount = totalAmount,
                    Status = OrderStatus.Pending,
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
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();

                throw;
            }
            
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            var response = new List<OrderResponseDto>();
            foreach(var order in orders)
            {
                response.Add(new OrderResponseDto
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    Items = order.OrderItems.Select(orderItem => new OrderItemResponseDto
                    {
                        OrderItemId = orderItem.Id,
                        ProductId = orderItem.ProductId,
                        ProductName = orderItem.Product.Name,
                        Quantity = orderItem.Quantity,
                        UnitPrice = orderItem.UnitPrice
                    }).ToList()
                });

            }
            return response;
        }

        public async Task<List<OrderResponseDto>> GetMyOrdersAsync(Guid userId)
        {
            var orders = await _orderRepository.GetOrderByUserIdAsync(userId);
            if(orders == null)
            {
                throw new NotFoundException("No orders were found!");
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
                throw new NotFoundException("No order found");
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

        public async Task<OrderResponseDto> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequestDto request)
        {
            //get order details
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException("No order found");
            }

            //validate status transition
            if(!IsValidStatusTransition(order.Status, request.Status))
            {
                throw new InvalidOperationException($"Cannot update from {order.Status} to {request.Status}");
            }

            order.Status = request.Status;
            await _unitOfWork.SaveChangesAsync();

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

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending =>
                    newStatus == OrderStatus.Confirmed ||
                    newStatus == OrderStatus.Declined ||
                    newStatus == OrderStatus.Cancelled,
                OrderStatus.Confirmed =>
                    newStatus == OrderStatus.Shipped ||
                    newStatus == OrderStatus.Cancelled,
                OrderStatus.Shipped => false,
                OrderStatus.Cancelled => false,
                OrderStatus.Declined => false,
                _ => false

            };
        }
    }
}
