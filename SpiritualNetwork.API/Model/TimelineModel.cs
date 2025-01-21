using SpiritualNetwork.Entities;
using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
    public class TimelineModel
    {
        public class PostCommentResponse
        {
            public string UserName { get; set; }
        }

        public class UserInfoResponse : UserPost
        {
            public string UserName { get; set; }
        }
        public class PostResponse
        {
            public int Id { get; set; }
            public string PostMessage { get; set; }
            public int PostUserId {  get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string ProfileImg { get; set; }
            public string Type { get; set; }
            public DateTime CreatedDate { get; set; }
            public bool isBookmarked { get; set; }
            public bool isFollowing { get; set; }
            public bool isLiked { get; set; }
            public bool? isPinPost { get; set; }
            public string? LastLiked { get; set; }
            public int? LikeCount { get; set; } 
        }

        public class CommentReposne
        {
            public int Id { get; set; }
            public string PostMessage { get; set; }
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime CreatedDate { get; set; }

        }

        public class UserChatResponse
        {
            public int UserId { get; set; }
            public string? UserName { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? ProfileImg { get; set; }
            public string? ConnectionId { get; set; }
            public int? IsSnooze { get; set; }
            public DateTime CreatedDate { get; set; }
            public string? Message { get; set; }
            public int UnReadCount { get; set; }
            public int? IsGroup { get; set; }

        }

        public List<PostResponse> Posts { get; set; }
    }
}
