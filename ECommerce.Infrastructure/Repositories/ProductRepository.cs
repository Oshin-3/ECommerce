using ECommerce.Application.DTOs.Common;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext _dbContext;
        public ProductRepository(ECommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Product>> GetAllProductsAsync(ProductQueryDto productQueryDto)
        {
            var query = _dbContext.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(productQueryDto.Search))
            {
                query = query.Where(p => 
                    p.Name.ToLower().Contains(productQueryDto.Search.ToLower()));
            }

            //check category has value
            if(productQueryDto.CategoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == productQueryDto.CategoryId.Value);
            }

            //min and max price
            if(productQueryDto.MinPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price >= productQueryDto.MinPrice.Value);
            }
            if (productQueryDto.MaxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price <= productQueryDto.MaxPrice.Value);
            }
            //sorting
            if(!string.IsNullOrEmpty(productQueryDto.SortBy))
            {
                var descending = productQueryDto.SortDirection?.ToLower() == "descending";

                query = productQueryDto.SortBy.ToLower() switch
                {
                    "name" => descending
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name),
                    "price" => descending
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price),
                    "stock" => descending
                        ? query.OrderByDescending(p => p.StockQuantity)
                        : query.OrderBy(p => p.StockQuantity),
                    _ => query.OrderBy(p => p.Name)
                };
            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }
            int skip = (productQueryDto.PageNumber - 1) * productQueryDto.PageSize;
            int take = productQueryDto.PageSize;
            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            //verify if the id is present in database
            var productId = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (productId == null)
                return null;
            return productId;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.Id = Guid.NewGuid();
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Guid id, Product product)
        {
            //verify if the product exists
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct == null)
                return null;
            //save in the database
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.CategoryId = product.CategoryId;

            //save changes
            await _dbContext.SaveChangesAsync();
            return existingProduct;

        }

        public async Task<Product> DeleteProductAsync(Guid id)
        {
            //verify if the product exists
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct == null)
                return null;

            //delete the product
            _dbContext.Products.Remove(existingProduct);
            await _dbContext.SaveChangesAsync();

            return existingProduct;

        }
    }

}
