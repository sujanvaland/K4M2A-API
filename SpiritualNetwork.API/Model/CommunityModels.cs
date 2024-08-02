using System.Globalization;

namespace SpiritualNetwork.API.Model
{
    public class CommunityModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Purpose { get; set; }
        public int ParentId { get; set; }
        public string? Type { get; set; }
        public string? ProfileImgUrl { get; set; }
        public string? BackgroundImgUrl { get; set; }
        public string? Question { get; set; }
        public int? TotalMembers { get; set; }
        public bool? IsJoined { get; set; }
        public bool? IsPending { get; set; }
        public bool? IsModerator { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public int? CreatedById { get; set; }
        public List<string>? MemberImgUrl { get; set; }
        public List<CommunityMemberModels>? Moderators { get; set; }
        public List<CommunityMemberModels>? MemberList { get; set; }
        public List<CommunityRulesModel>? Rules { get; set; }
        public bool IsNotification { get; set; }
        public int? ReportPostCount { get; set; }
        public int? MemberReqCount { get; set; }
    }

    public class CommunityReportPostModel
    {
        public int PostId { get; set; }
        public string Type { get; set; }
        public int CommunityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? About { get; set; }
        public string? ProfileImgUrl { get; set; }
        public List<CommunityMemberModels>? Members { get; set; }
    }

    public class AddRevomeMember
    {
        public string? Action { get; set; }
        public List<int>? UserId { get; set; }
        public int CommunityId { get; set; }
    }

    public class DeleteRules
    {
        public int Id { get; set; }
        public int CommunityId { get; set; }
    }

    public class JoinLeaveReq
    {
        public int UserId { get; set; }
        public int CommunityId { get; set; }
        public string? Answer { get; set; }
    }
    public class MemberListReq
    {
        public int CommunityId { get; set; }
        public int UserId { get; set; }
    }

    public class CommunityMemberModels
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? About { get; set; }
        public string? ProfileImgUrl { get; set;}
        public bool IsModerator { get; set; }
        public bool IsPremium { get; set; }
        public bool  IsFollowing { get; set;}
        public bool IsFollower { get; set; }
        public string? Answer { get; set; }
        public bool? IsBusinessAccount { get; set; }

    }

    public class CommunityRulesModel
    {
        public int Id { get; set; }
        public string Rules { get; set; }
        public string? Descriptions { get; set; }
    }

    public class ExploreCommunityReq
    {
        public int PageNo { get; set; }
        public string? Search { get; set; }

    }

    public class CommunityReportPostReq
    {
        public int CommunityId { get; set; }
        public string ReportDetails { get; set;}
        public int PostId { get; set; }
        public string Type { get; set; }
    }

    public class CommunityReportPostAction
    {
        public int PostId { get; set; }
        public int CommunityId { get; set; }
        public string? ActionType { get; set; }
    }
}
