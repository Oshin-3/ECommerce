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
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _dbContext.Products.ToListAsync();
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
