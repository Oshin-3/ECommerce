using ECommerce.Application.DTOs.CartItem;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Cart
{
    public class GetCartResponseDto
    {
        public Guid CartId { get; set; }

        public List<CartItemResponseDto> Items { get; set; }
        public decimal TotolAmount { get; set; } 
    }
}
