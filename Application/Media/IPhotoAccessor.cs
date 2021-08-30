using Microsoft.AspNetCore.Http;

namespace Application.Media
{
    public interface IPhotoAccessor
    {
        PhotoUploadResult AddPhoto(IFormFile file);
        string DeletePhoto(string publicId);
    }
}
