using AutoMapper;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.DTOs.Product;
using ECommerce.Domain.Entities;

namespace ECommerce.API.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            #region Category
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, AddCategoryRequestDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryRequestDto>().ReverseMap();
            #endregion

            #region Product
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, AddProductRequestDto>().ReverseMap();
            CreateMap<Product, UpdateProductRequestDto>().ReverseMap();
            #endregion
        }
    }
}
