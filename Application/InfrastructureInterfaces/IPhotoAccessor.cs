using System.Threading.Tasks;
using Application.InfrastructureModels;
using Microsoft.AspNetCore.Http;

namespace Application.InfrastructureInterfaces
{
    public interface IPhotoAccessor
    {
        Task<PhotoUploadResult> AddPhotoAsync(IFormFile file);
        Task<bool> DeletePhotoAsync(string publicId);
    }
}
