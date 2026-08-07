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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ECommerceDbContext _dbContext;
        public CategoryRepository(ECommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            category.Id = Guid.NewGuid();
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            //verify if the category is present
            var categoryId = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoryId == null)
                return null;
            return categoryId;
        }

        public async Task<Category> UpdateCategoryAsync(Guid id, Category category)
        { 
            //if the record exists or not
            var existingCategory = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existingCategory == null)
            {
                return null;
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            //save changes
            await _dbContext.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<Category> DeleteCategoryAsync(Guid id)
        {
            //first check if the record exists
            var categoryById = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (categoryById == null)
            {
                return null;
            }

            //remove the record 
            _dbContext.Categories.Remove(categoryById);
            await _dbContext.SaveChangesAsync();

            return categoryById;
        }
    }
}
