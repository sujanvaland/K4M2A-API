using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IActivityLogService
    {
        public Task<JsonResponse> SaveUserActivity(ActivityModel activityLog);
        public Task<JsonResponse> GetSearchKeywordsAndUsers(int UserId);
        public Task<JsonResponse> DeleteSearchKeywordsAndUsers(int Id, int UserId, string message, string type);


    }
}
