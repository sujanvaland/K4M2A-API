using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ActivityController : ApiBaseController
    {
        private readonly IActivityLogService _activityService;

        public ActivityController(IActivityLogService activityService)
        {
            _activityService = activityService;
        }


        [HttpPost(Name = "SaveUserActivity")]
        public async Task<JsonResponse> SaveUserActivity(ActivityModel req)
        {
            try
            {
                req.UserId = user_unique_id;
                var response = await _activityService.SaveUserActivity(req);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }


        [HttpGet(Name = "GetRecentSearch")]
        public async Task<JsonResponse> GetRecentSearch()
        {
            try
            {
                var response = await _activityService.GetSearchKeywordsAndUsers(user_unique_id);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "DeleteRecentSearch")]
        public async Task<JsonResponse> DeleteRecentSearch(DeleteReq req)
        {
            try
            {
                var response = await _activityService.DeleteSearchKeywordsAndUsers(req.Id,user_unique_id,req.Message,req.Type);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
