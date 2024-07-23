using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PollController : ApiBaseController
    {
        private readonly IPollService _pollService;
        public PollController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpPost(Name = "SavePoll")]
        public async Task<JsonResponse> SavePoll(Poll poll)
        {
            try
            {
                var response = await _pollService.SavePoll(poll);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "SavePollVote")]
        public async Task<JsonResponse> SavePollVote(PollVote vote)
        {
            try
            {
                vote.UserId = user_unique_id;
                var response = await _pollService.SavePollVote(vote);
                return new JsonResponse(200,true,"Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetPollDetails")]
        public async Task<JsonResponse> GetPollDetails(int PollId)
        {
            try
            {
                var response = await _pollService.GetPollDetails(PollId,user_unique_id);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }

}
