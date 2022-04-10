namespace Bookworm.Services.Data.Models
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Bookworm.Services.Data.Contracts;
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    public class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration configuration;

        public CloudinaryService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string publicId)
        {
            Cloudinary cloudinary = new(this.configuration.GetValue<string>("Cloudinary:CloudinaryUrl"));
            using Stream stream = imageFile.OpenReadStream();
            ImageUploadParams uploadParams = new()
            {
                File = new FileDescription(imageFile.FileName, stream),
                PublicId = publicId,
            };

            ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);

            string imageUrl = uploadResult.SecureUrl.AbsoluteUri;

            return imageUrl;
        }

        public async Task DeleteImage(string publicId)
        {
            Cloudinary cloudinary = new(this.configuration.GetValue<string>("Cloudinary:CloudinaryUrl"));
            var delParams = new DelResParams()
            {
                PublicIds = new List<string>() { publicId },
                Invalidate = true,
            };

            await cloudinary.DeleteResourcesAsync(delParams);
        }
    }
}
