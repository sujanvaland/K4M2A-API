using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]")]
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
    }
}
