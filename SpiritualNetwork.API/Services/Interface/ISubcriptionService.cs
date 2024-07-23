using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface ISubcriptionService
    {
        public Task<JsonResponse> SaveSubcription(SubcriptionModel res, int userId);
    }
}
