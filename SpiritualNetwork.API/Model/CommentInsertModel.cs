using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
    public class CommentInsertModel
    {
        public int UserId { get; set; }
        [MaxLength(2000)]
        public string PostMessage { get; set; }
        [MaxLength(10)]
        public string Type { get; set; }
        public int? ParentId { get; set; }
        public string? Category { get; set; }
        public int? PostId { get; set; }
    }

    public class LikeListResponse
    {
        public int PostId { get; set; }
        public int LikeCount { get; set; }
        public List<LikeList> UserList { get; set; }
    }
    public class LikeList
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string? ProfileImg { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsDisplay { get; set;}
        public bool? IsBusinessAccount { get; set; }

    }
}
