﻿namespace SpiritualNetwork.API.Model
{
    public class ActivityModel
    {
        public int UserId { get; set; }
        public string? ActivityType { get; set; }
        public string? Type { get; set; }
        public int? RefId1 { get; set; }
        public int? RefId2 { get; set; }
        public string? Message { get; set; }
        public string? IPAddress { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }

    public class SearchResponse
    {
        public List<SearchedKeywordsRes> RecentSearchedKeywords { get; set; }
        public List<SearchedUserRes> RecentSearchedUsers { get; set; }
    }

    public class SearchedUserRes
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? ProfileImgUrl { get; set; }
        public bool? IsBusinessAccount { get; set; }

    }

    public class SearchedKeywordsRes
    {
        public int Id { get; set; }
        public string Keywords { get; set; }
    }

    public class DeleteReq
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }

    public class ContactUserRes
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? ProfileImg { get; set; }
        public bool? IsBusinessAccount { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email {  get; set; } 

    }

    public class InviteUserRes
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Photo { get; set; }

    }


    public class InviteContactRes
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Photo { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
