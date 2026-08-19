using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class UpdateProductImageService : IUpdateProductImageService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateProductImageService(IProductRepository productRepository,
                IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task UpdateProductImageAsync(Guid productId, string imageUrl)
        {
            //get product details first
            var product = await _productRepository.GetProductByIdAsync(productId);
            if(product == null)
            {
                throw new NotFoundException("Product not found");
            }

            product.ImageUrl = imageUrl;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
