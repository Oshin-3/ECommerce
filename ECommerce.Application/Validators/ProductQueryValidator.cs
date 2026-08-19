using ECommerce.Application.DTOs.Product;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Validators
{
    public class ProductQueryValidator: AbstractValidator<ProductQueryDto>
    {
        public ProductQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page sixe must be between 1 to 100.");

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinPrice.HasValue)
                .WithMessage("Minimum price cannot be negative.");

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxPrice.HasValue)
                .WithMessage("Maximum price cannot be negative.");

            RuleFor(x => x)
                .Must(x =>
                    !x.MinPrice.HasValue ||
                    !x.MaxPrice.HasValue ||
                    x.MinPrice <= x.MaxPrice)
                .WithMessage("Minimum price cannot be greater than maximum price.");

            RuleFor(x => x.SortDirection)
                .Must(value =>
                    string.IsNullOrWhiteSpace(value) ||
                    value.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                    value.Equals("dsc", StringComparison.OrdinalIgnoreCase))
                .WithMessage("SortDirection must be asc or desc.");

            RuleFor(x => x.SortBy)
                .Must(value =>
                    string.IsNullOrWhiteSpace(value) ||
                    new[] { "name", "price", "stock" }
                        .Contains(value.ToLower()))
                .WithMessage("SortBy must be name, price, stock.");
        }
    }
}
