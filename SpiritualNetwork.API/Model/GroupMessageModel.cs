using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class GroupMessageModel
    {
        public int GroupId { get; set; }
        public string? Name { get; set; }
        public List<int> userId { get; set; }
    }

    public class UpdateGroupDetails
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string ProfileImgUrl { get; set; }
    }

    public class DeleteMessage
    {
        public int Id { get; set; }
        public int MessageId { get; set; } = 0;
        public int TimeStamp { get; set; } = 0;
    }

    public class leaveGroupModel
    {
        public int GroupId { get; set; }
    }
    public class InfoBoxModel
    {
        public int Id { get; set; }
        public bool IsGroup { get; set; }
    }
    public class SnoozeUserGroupReq
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public bool IsGroup { get; set; }
    }
    public class GroupInfoDetails
    {
        public List<GroupInfoBoxModel> GroupInfoBoxModel { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsNotification { get; set; }
        public bool IsMention { get; set; }
    }

    public class GroupInfoBoxModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string ProfileImgUrl { get; set; }
        public bool IsAdmin { get; set; }
        public int isfollowing { get; set; }
        public bool isPremium { get; set; }
        public int? isPeer { get; set; }
        public bool? IsBusinessAccount { get; set; }

    }

    public class ActionUserFGroup
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public string? Type { get; set; }
    }

    public class MentionBoxModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string About { get; set; }
        public string ProfileImgUrl { get; set; }
        public int NoOfFollowing { get; set; }
        public int NoOfFollowers { get; set; }
        public int isfollowing { get; set; }
        public int? isPeer { get; set; }

        public bool isPremium { get; set; }
        public bool? IsBusinessAccount { get; set; }

    }
}
