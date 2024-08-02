namespace SpiritualNetwork.API.Model
{
    public class AdminResponseModel
    {
        public int UserCount { get; set; }
        public int LikeCount { get; set; }
        public int EventCount { get; set; }
        public int CommunityCount { get; set; }
        public int CommentCount { get; set; }
        public int PostCount { get; set; }
    }


    public class ReportReq
    {
        public string Type { get; set; } 
    }
    public class ReportDetailReq
    {
        public int ReportedId { get; set; }
        public string Type { get; set; }
    }

    public class ReportDetailsResponse
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; } 
        public string? ProfileImgUrl { get; set; }
        public bool? IsBusinessAccount { get; set; }

    }

    public class IdReq
    {
        public int Id { get; set; }
    }

    public class UserListResponse
    {
        public int UserCount { get; set; }
        public List<SearchUserResModel> searchUserResModel { get; set; }
    }


    public class ReportResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImgUrl { get; set; }
        public int ReportedId { get; set; }
        public string Type { get; set; }
        public int? ReportCount { get; set; }
        public bool? IsBusinessAccount { get; set; }


    }

}
