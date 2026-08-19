using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ECommerce.Application.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Domain.Contants;
using ECommerce.Application.DTOs.Common;
using FluentValidation;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ProductQueryDto> _validator;
        private readonly IProductImageService _productImageService;
        private readonly IUpdateProductImageService _productService;
        public ProductController(IProductRepository productRepository, 
            IMapper mapper,
            IValidator<ProductQueryDto> validator,
            IProductImageService productImageService,
            IUpdateProductImageService productService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _validator = validator;
            _productImageService = productImageService;
            _productService = productService;

        }

        #region Upload Product Image
        [HttpPost]
        [Route("{productId:guid}/image")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UploadProductImage(Guid productId, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Please select an image");
            }

            await using var stream = image.OpenReadStream();
            var imageUrl = await _productImageService.UploadImageAsync(
                    stream, image.FileName, image.ContentType);

            await _productService.UpdateProductImageAsync(productId, imageUrl);

            return Ok(new
            {
                ImageUrl = imageUrl
            });
        }

        #endregion

        #region Get All Products
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryDto productQueryDto)
        {
            //validate
            var validationResult = await _validator.ValidateAsync(productQueryDto);
            if(!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors
                    .Select(e => e.ErrorMessage));
            }
            //retrive details from database 
            var allProducts = await _productRepository.GetAllProductsAsync(productQueryDto);

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
