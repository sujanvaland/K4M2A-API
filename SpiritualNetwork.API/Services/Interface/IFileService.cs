using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IFileService
    {
        public Task<List<Entities.File>> UploadFile(IFormCollection form);
        public Task<List<Entities.File>> UploadFileForSuggestion(IFormFile form, string path);
        public Task<JsonResponse> DeleteUploadedFile(int Id, string Url);


    }
}
