using AutoMapper;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Contants;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        #region Create Category
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [Route("create")]
        public async Task<IActionResult> CreateCategory([FromBody] AddCategoryRequestDto addcategoryRequestDto)
        {
            //map Dto to domain model
            //var category = new Domain.Category
            //{
            //    Name = addcategoryRequestDto.Name,
            //    Description = addcategoryRequestDto.Description

            //};
            var category = _mapper.Map<Category>(addcategoryRequestDto);

            // Save the product to the database
            await _categoryRepository.CreateCategoryAsync(category);

            //map domain model to Dto
            //var categoryDto = new CategoryDto
            //{
            //    Id = category.Id,
            //    Name = category.Name,
            //    Description = category.Description
            //};
            var categoryDto = _mapper.Map<CategoryDto>(category);

            return Ok(categoryDto);
        }
        #endregion

        #region Get All Categories
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCategories()
        {
            // Retrieve all categories from the database
            var category = await _categoryRepository.GetAllCategoriesAsync();

            //convert domain model to Dto
            //var categoryDto = new List<CategoryDto>();
            //foreach(var item in category)
            //{
            //    categoryDto.Add(new CategoryDto
            //    {
            //        Id = item.Id,
            //        Name = item.Name,
            //        Description = item.Description
            //    });
            //}
            var categoryDto = _mapper.Map<List<CategoryDto>>(category);
            return Ok(categoryDto);
            
        }
        #endregion

        #region Get Category By Id
        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            //retrive data from database
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            //convert domain model to Dto 
            //var categoryDto = new CategoryDto
            //{
            //    Id = category.Id,
            //    Name = category.Name,
            //    Description = category.Description

            //};
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }
        #endregion

        #region Update Category
        [HttpPut]
        [Authorize(Roles = Roles.Admin)]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryRequestDto updateCategoryRequestDto)
        {
            //convert dto to domain model
            //var category = new Domain.Category
            //{
            //    Name = updateCategoryRequestDto.Name,
            //    Description = updateCategoryRequestDto.Description
            //};
            var category = _mapper.Map<Category>(updateCategoryRequestDto);

            //update the category in the database
            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(id, category);
            if (updatedCategory == null)
            {
                return NotFound();
            }

            //convert domain model to dto
            //var updatedCategoryDto = new CategoryDto
            //{
            //    Id = updatedCategory.Id,
            //    Name = updatedCategory.Name,
            //    Description = updatedCategory.Description
            //};
            var updatedCategoryDto = _mapper.Map<CategoryDto>(updatedCategory);
            return Ok(updatedCategoryDto);
        }
        #endregion

        #region Delete Category
        [HttpDelete]
        [Authorize(Roles = Roles.Admin)]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            //retrive the category from the database
            var category = await _categoryRepository.DeleteCategoryAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok();
        }
        #endregion
    }
}
