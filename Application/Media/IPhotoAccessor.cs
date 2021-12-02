using Microsoft.AspNetCore.Http;

namespace Application.Media;

public interface IPhotoAccessor
{
    Task<PhotoUploadResult> AddPhotoAsync(IFormFile file);
    Task<bool> DeletePhotoAsync(string publicId);
}

