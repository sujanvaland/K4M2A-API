namespace SpiritualNetwork.Entities
{
    public class UserMuteBlockList : BaseEntity
    {
        public int UserId { get; set; }
        public int BlockedUserId { get; set; }
        public int MuteedUserId { get; set; }
    }
}
