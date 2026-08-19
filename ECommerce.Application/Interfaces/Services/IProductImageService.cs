using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IProductImageService
    {
        Task<string> UploadImageAsync(
            Stream fileStream,
            string fileName,
            string contentType);

        ///Task DeleteImageAsync(string imageUrl);
    }
}
