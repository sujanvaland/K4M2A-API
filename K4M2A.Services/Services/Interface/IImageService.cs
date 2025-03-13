using Microsoft.AspNetCore.Http;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface IImageService
    {
        public Task<JsonResponse> GetThumbNail(IFormFile file);
        public Task<JsonResponse> GetThumbnailFile(byte[] bytearr);
    }
}
