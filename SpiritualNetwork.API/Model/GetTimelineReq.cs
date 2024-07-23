namespace SpiritualNetwork.API.Model
{
    public class GetTimelineReq
    {
        public int PageNo { get; set; }
        public int? ParentId { get; set; }
        public int? ProfileUserId { get; set; }
        public string? Type { get; set; }
    }

    public class ExtractUrlMetaReq
    {
        public string Url { get; set; }
    }

    public class MentionListReq
    {
        public List<string> userName { get; set; }
    }
}
