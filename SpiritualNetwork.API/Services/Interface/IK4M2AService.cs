using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IK4M2AService
    {
        public Task<JsonResponse> SaveServiceDetails(ServiceReq req);
        public Task<JsonResponse> UpdateServiceFAQ(ServiceFAQReq req);
        public Task<JsonResponse> DeleteServiceFAQ(ServiceIdReq req);
        public Task<JsonResponse> GetServiceById(ServiceIdReq req);
        public Task<JsonResponse> DeleteService(ServiceIdReq req);
        public Task<JsonResponse> GetAllService(GetServiceReq req);
    }
}
