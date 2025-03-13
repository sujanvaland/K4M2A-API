using K4M2A.Entities.Model;
using K4M2A.Entities;
using K4M2A.Entities.CommonModel;

namespace K4M2A.API.Services.Interface
{
    public interface ICommunityService
    {
        public Task<Community> AddUpdateCommunity(Community req);
        public Task<JsonResponse> AddUpdateCommunityMember(AddRevomeMember req, int UserId);
        public Task<JsonResponse> AddUpdateRules(CommunityRules req, int UserId);
        public Task<JsonResponse> JoinLeaveCommunity(JoinLeaveReq req);
        public Task<JsonResponse> MyCommunityList(int UserId);
        public Task<JsonResponse> ExploreCommunityList(ExploreCommunityReq req, int Size, int UserId);
        public Task<JsonResponse> GetCommunityById(int CommunityId, int UserId);
        public Task<JsonResponse> AllMemberList(MemberListReq req);
        public Task<JsonResponse> AllModeratorList(MemberListReq req);
        public Task<JsonResponse> MemberReqList(MemberListReq req);
        public Task<JsonResponse> DeleteRules(DeleteRules req, int UserId);
        public Task<JsonResponse> ReportCommunityPost(CommunityReportPostReq req, int UserId);
        public Task<JsonResponse> ActionCommunityReportPost(CommunityReportPostAction req, int UserId);
        public Task<JsonResponse> GetAllCommunityReportPost(int CommunityId, int UserId);
        public Task<JsonResponse> DeleteCommunity(int CommunityId, int UserId);

    }
}
