using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Test.Services
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();

            _cartService = new CartService(
                _cartRepositoryMock.Object,
                _productRepositoryMock.Object);
        }

        [Fact]
        public async Task AddToCartAsync_WhenProductExistsAndItemNotInCart_ShouldAddNewCartItem()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            var request = new AddToCartRequestDto
            {
                ProductId = productId,
                Quantity = 2
            };

            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 1000
            };

            var cart = new Cart
            {
                Id = cartId,
                UserId = userId
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            _cartRepositoryMock
                .Setup(x => x.GetCartByUserIdAsync(userId))
                .ReturnsAsync(cart);

            _cartRepositoryMock
                .Setup(x => x.GetCartItemAsync(cartId, productId))
                .ReturnsAsync((CartItem?)null);

            // Act
            await _cartService.AddToCartAsync(userId, request);

            // Assert
            _cartRepositoryMock.Verify(
                x => x.AddCartItemAsync(
                    It.Is<CartItem>(ci =>
                        ci.CartId == cartId &&
                        ci.ProductId == productId &&
                        ci.Quantity == 2)),
                Times.Once);

            _cartRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task AddToCartAsync_WhenItemAlreadyExists_ShouldIncreaseQuantity()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            var request = new AddToCartRequestDto
            {
                ProductId = productId,
                Quantity = 2
            };

            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 1000
            };

            var cart = new Cart
            {
                Id = cartId,
                UserId = userId
            };

            var existingCartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductId = productId,
                Quantity = 3
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            _cartRepositoryMock
                .Setup(x => x.GetCartByUserIdAsync(userId))
                .ReturnsAsync(cart);

            _cartRepositoryMock
                .Setup(x => x.GetCartItemAsync(cartId, productId))
                .ReturnsAsync(existingCartItem);

            // Act
            await _cartService.AddToCartAsync(userId, request);

            // Assert
            Assert.Equal(5, existingCartItem.Quantity);

            _cartRepositoryMock.Verify(
                x => x.AddCartItemAsync(It.IsAny<CartItem>()),
                Times.Never);

            _cartRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]

        public async Task AddToCartAsync_WhenQuantityIsZero_ShouldThrowBusinessRuleException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new AddToCartRequestDto
            {
                ProductId = Guid.NewGuid(),
                Quantity = 0
            };

            // Act
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                () => _cartService.AddToCartAsync(userId, request));

            // Assert
            Assert.Equal(
                "Quantity must be greater than zero.",
                exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5)]
        [InlineData(-100)]
        public async Task AddToCartAsync_WhenQuantityIsInvalid_ShouldThrowBusinessRuleException(
    int quantity)
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new AddToCartRequestDto
            {
                ProductId = Guid.NewGuid(),
                Quantity = quantity
            };

            // Act
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(
                () => _cartService.AddToCartAsync(userId, request));

            // Assert
            Assert.Equal(
                "Quantity must be greater than zero.",
                exception.Message);

            _productRepositoryMock.Verify(
                x => x.GetProductByIdAsync(It.IsAny<Guid>()),
                Times.Never);

            _cartRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task AddToCartAsync_WhenProductDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var request = new AddToCartRequestDto
            {
                ProductId = productId,
                Quantity = 2
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync((Product?)null);

            // Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _cartService.AddToCartAsync(userId, request));

            // Assert
            Assert.Equal("No Product found!", exception.Message);

            _cartRepositoryMock.Verify(
                x => x.GetCartByUserIdAsync(It.IsAny<Guid>()),
                Times.Never);

            _cartRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task AddToCartAsync_WhenCartDoesNotExist_ShouldCreateCartAndAddItem()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var request = new AddToCartRequestDto
            {
                ProductId = productId,
                Quantity = 2
            };

            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 1000
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            _cartRepositoryMock
                .Setup(x => x.GetCartByUserIdAsync(userId))
                .ReturnsAsync((Cart?)null);

            _cartRepositoryMock
                .Setup(x => x.GetCartItemAsync(
                    It.IsAny<Guid>(),
                    productId))
                .ReturnsAsync((CartItem?)null);

            // Act
            await _cartService.AddToCartAsync(userId, request);

            // Assert
            _cartRepositoryMock.Verify(
                x => x.CreateCartAsync(
                    It.Is<Cart>(cart =>
                        cart.UserId == userId)),
                Times.Once);

            _cartRepositoryMock.Verify(
                x => x.AddCartItemAsync(
                    It.Is<CartItem>(item =>
                        item.ProductId == productId &&
                        item.Quantity == 2)),
                Times.Once);

            _cartRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

    }
}
