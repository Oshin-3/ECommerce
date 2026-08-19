using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.API.Services
{
    public class LocalProductImageService : IProductImageService
    {
        private readonly IWebHostEnvironment _environment;
        public LocalProductImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
        {
            //allow image type
            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if(!allowedTypes.Contains(contentType.ToLower()))
            {
                throw new BusinessRuleException("Only JPEG, PNG and WEBP images are allowed.");
            }

            //create folder path
            var folderPath = Path.Combine(_environment.WebRootPath,
                "images",
                "products");
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            //create a unique filename so two users dont overwrite each others images
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            //create full physical path
            var filePath = Path.Combine(folderPath, uniqueFileName);

            //save the stream
            await using var outputStream = new FileStream(filePath,
                FileMode.Create);
            await fileStream.CopyToAsync(outputStream);

            return $"/images/products/{uniqueFileName}";
        }
    }
}
