using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IUpdateProductImageService
    {
        Task UpdateProductImageAsync(Guid productId, string imageUrl);
    }
}
