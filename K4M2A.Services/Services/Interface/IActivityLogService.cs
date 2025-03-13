using K4M2A.Entities.Model;
using K4M2A.Entities.CommonModel;

namespace K4M2A.Services.Interface
{
    public interface IActivityLogService
    {
        public Task<JsonResponse> SaveUserActivity(ActivityModel activityLog);
        public Task<JsonResponse> GetSearchKeywordsAndUsers(int UserId);
        public Task<JsonResponse> DeleteSearchKeywordsAndUsers(int Id, int UserId, string message, string type);


    }
}
