namespace K4M2A.Entities
{
    public class UserFollowers: BaseEntity
    {
        public int UserId { get; set; }
        public int FollowToUserId { get; set; }
    }
}
