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

    public class CommunityController : ApiBaseController
    {
      
        private readonly ICommunityService _communityService;

        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        [HttpPost(Name = "AddUpdateCommunity")]
        public async Task<JsonResponse> AddUpdateCommunity(Community req)
        {
            try
            {
                req.CreatedBy = user_unique_id;
                var response = await _communityService.AddUpdateCommunity(req);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "AddRemoveCommunityMember")]
        public async Task<JsonResponse> AddRemoveCommunityMember(AddRevomeMember req)
        {
            try
            {
                return await _communityService.AddUpdateCommunityMember(req,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "AddUpdateCommunityRules")]
        public async Task<JsonResponse> AddUpdateCommunityRules(CommunityRules req)
        {
            try
            {
                return await _communityService.AddUpdateRules(req, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "ReportCommunityPost")]
        public async Task<JsonResponse> ReportCommunityPost(CommunityReportPostReq req)
        {
            try
            {
                return await _communityService.ReportCommunityPost(req, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "ActionCommunityReportPost")]
        public async Task<JsonResponse> ActionCommunityReportPost(CommunityReportPostAction req)
        {
            try
            {
                return await _communityService.ActionCommunityReportPost(req, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetAllCommunityReportPost")]
        public async Task<JsonResponse> GetAllCommunityReportPost(int CommunityId)
        {
            try
            {
                return await _communityService.GetAllCommunityReportPost(CommunityId, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "JoinLeaveCommunity")]
        public async Task<JsonResponse> JoinLeaveCommunity(JoinLeaveReq req)
        {
            try
            {
                req.UserId = user_unique_id;
                return await _communityService.JoinLeaveCommunity(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetCommunityById")]
        public async Task<JsonResponse> GetCommunityById(int id)
        {
            try
            {
                return await _communityService.GetCommunityById(id,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "MyCommunityList")]
        public async Task<JsonResponse> MyCommunityList()
        {
            try
            {
                return await _communityService.MyCommunityList( user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "ExploreCommunityList")]
        public async Task<JsonResponse> ExploreCommunityList(ExploreCommunityReq req)
        {
            try
            {
                return await _communityService.ExploreCommunityList(req,30,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetMemberList")]
        public async Task<JsonResponse> GetMemberList(MemberListReq req)
        {
            try
            {
                req.UserId = user_unique_id;
                return await _communityService.AllMemberList(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetModeratorList")]
        public async Task<JsonResponse> GetModeratorList(MemberListReq req)
        {
            try
            {
                req.UserId = user_unique_id;
                return await _communityService.AllModeratorList(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetMemberReqList")]
        public async Task<JsonResponse> GetMemberReqList(MemberListReq req)
        {
            try
            {
                req.UserId = user_unique_id;
                return await _communityService.MemberReqList(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "DeleteCommunityRules")]
        public async Task<JsonResponse> DeleteCommunityRules(DeleteRules req)
        {
            try
            {
                return await _communityService.DeleteRules(req,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "DeleteCommunity")]
        public async Task<JsonResponse> DeleteCommunity(int CommunityId)
        {
            try
            {
                return await _communityService.DeleteCommunity(CommunityId, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
