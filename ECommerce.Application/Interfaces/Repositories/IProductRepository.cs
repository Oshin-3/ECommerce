using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Product;
using ECommerce.Domain;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync(PaginationQueryDto paginationDto);
        Task<Product> GetProductByIdAsync(Guid id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Guid id, Product product);
        Task<Product> DeleteProductAsync(Guid id);
    }
}
