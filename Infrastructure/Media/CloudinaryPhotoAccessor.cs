using System;
using System.Threading.Tasks;
using Application.InfrastructureInterfaces;
using Application.InfrastructureModels;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Media
{
    public class CloudinaryPhotoAccessor : IPhotoAccessor
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryPhotoAccessor(IOptions<CloudinarySettings> settings)
        {
            var acc = new Account(
                settings.Value.CloudName,
                settings.Value.APIKey,
                settings.Value.APISecret
                );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<PhotoUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(1000).Width(1000).Crop("fill")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            return new PhotoUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.AbsoluteUri
            };
        }

        public async Task DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
                throw new Exception(result.Error.Message);
        }
    }
}
