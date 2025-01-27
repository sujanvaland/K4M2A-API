namespace SpiritualNetwork.API.Model
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
        public List<string> RecentSearchedKeywords { get; set; }
        public List<SearchedUserRes> RecentSearchedUsers { get; set; }
    }

    public class SearchedUserRes
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? ProfileImgUrl { get; set; }
        public bool? IsBusinessAccount { get; set; }

    }

}
