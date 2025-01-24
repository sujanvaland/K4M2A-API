using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IActivityLogService
    {
        public Task<JsonResponse> SaveUserActivity(ActivityModel activityLog);

    }
}
