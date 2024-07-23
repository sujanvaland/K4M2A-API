using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IImageService
    {
        public Task<JsonResponse> GetThumbNail(IFormFile file);
        public Task<JsonResponse> GetThumbnailFile(byte[] bytearr);
    }
}
