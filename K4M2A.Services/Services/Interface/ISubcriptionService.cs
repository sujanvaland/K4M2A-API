using K4M2A.Entities.Model;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface ISubcriptionService
    {
        public Task<JsonResponse> SaveSubcription(SubcriptionModel res, int userId);
    }
}
