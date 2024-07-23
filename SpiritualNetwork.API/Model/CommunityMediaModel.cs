namespace SpiritualNetwork.API.Model
{
    public class CommunityMediaModel
    { 
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? ImgUrl { get; set; }
        public string? VideoUrl { get; set; }
        public int? CommunityId { get; set; }

    }
}
