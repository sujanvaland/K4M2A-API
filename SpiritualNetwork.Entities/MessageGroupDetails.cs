
namespace SpiritualNetwork.Entities
{
    public class MessageGroupDetails : BaseEntity
    {
        public string GroupName { get; set; }
        public string ProfileImgUrl{ get; set; }
    }

    public class GroupMember : BaseEntity
    {
        public int GroupId { get; set; }
        public int UserId  { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsAdmin { get; set; }
        public int IsNotification { get; set; }
        public int IsMention { get; set; }
    }

    public class SnoozeUser : BaseEntity
    {
        public int UserId { get; set; }
        public int SnoozeUserId { get; set; }
        public int IsNotification { get; set; }
    }
}
