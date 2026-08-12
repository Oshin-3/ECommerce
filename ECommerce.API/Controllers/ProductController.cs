using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ECommerce.Application.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Domain.Contants;
using ECommerce.Application.DTOs;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        readonly IProductRepository _productRepository;
        readonly IMapper _mapper;
        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        #region Get All Products
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProducts([FromQuery] PaginationQueryDto productPaginationDto)
        {
            //retrive details from database 
            var allProducts = await _productRepository.GetAllProductsAsync(productPaginationDto);

            //convert domain model to Dto
            var allProductsDto = _mapper.Map<List<ProductDto>>(allProducts);

            return Ok(allProductsDto);
        }
        #endregion

        #region Get Product By Id
        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            //retrive the data from database
            var productId = await _productRepository.GetProductByIdAsync(id);
            if (productId == null)
                return NotFound();

            //convert domain model to Dto
            var productIdDto = _mapper.Map<ProductDto>(productId);
            return Ok(productIdDto);
        }

        #endregion

        #region Create Product
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [Route("create")]
        public async Task<IActionResult> CreateProduct([FromBody] AddProductRequestDto addProductRequestDto)
        {
            //convert dto to domain model
            var newProduct = _mapper.Map<Product>(addProductRequestDto);

            //add the product to database
            newProduct = await _productRepository.CreateProductAsync(newProduct);

            //convert domail model to dto
            var newProductDto = _mapper.Map<ProductDto>(newProduct);
            return Ok(newProductDto);
        }
        #endregion

        #region Update Product
        [HttpPut]
        [Authorize(Roles = Roles.Admin)]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductRequestDto updateProductRequestDto)
        {
            //convert dto to domain model
            var updatedProduct = _mapper.Map<Product>(updateProductRequestDto);

            //update in the database
            updatedProduct = await _productRepository.UpdateProductAsync(id, updatedProduct);
            if (updatedProduct == null)
                return NotFound();

            //convert domain model to dto
            var updatedProductDto = _mapper.Map<ProductDto>(updatedProduct);
            return Ok(updatedProductDto);
        }
        #endregion

        #region Delete Product
        [HttpDelete]
        [Authorize(Roles = Roles.Admin)]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            //retrive data from database
            var existingProduct = await _productRepository.DeleteProductAsync(id);
            if (existingProduct == null)
                return NotFound();

            return Ok();
        }


        #endregion

    }
}
