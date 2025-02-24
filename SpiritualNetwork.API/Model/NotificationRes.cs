﻿using SpiritualNetwork.Entities;
using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
    public class NotificationRes
    {
        public NotificationRes()
        {
            connectionIds = new List<string>();
        }
        public int PostId { get; set; }
        public int ActionByUserId { get; set; }
        public string ActionType { get; set; }
        public string RefId1 { get; set; }
        public string RefId2 { get; set; }
        public string Message { get; set; }
        public string PushAttribute { get; set; }
        public string EmailAttribute { get; set; }
        public List<string> connectionIds { get; set; }
    }

    public class userNotificationRes
    {
        public int NotificationId { get; set; }
        public UserDetails UserDetail { get; set; }
        public List<UserDetails> UserDetailList { get; set; }
        public int OtherLikeCount { get; set; } = 0;
        public string Type { get; set; }
        public string Message { get; set; }
        public int? PostId { get; set; }
        public int? ParentPostId { get; set; }
        public UserDetails? RepostUserDetail { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsRead { get; set; }
    }

    public class UserDetails
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? About { get; set; }
        public string? ProfileImg { get; set; }
        public bool? IsBusinessAccount { get; set; }

    }

    public class NotificationCountRes
    {
        public int Message { get; set; }
        public int ReportCommunityPost { get; set; }
        public int CommunityRequest { get; set; }
        public int EventRequest { get; set; }
        public int Notification { get; set; }
    }

    public class NodeAddPost
    {
        public int Id { get; set; }
    }

}
