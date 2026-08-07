using ECommerce.Application.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(Guid userId, AddToCartRequestDto request);
    }
}
