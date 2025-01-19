namespace SpiritualNetwork.API.Model
{
    public class Report
    {
        public string Type { get; set; }
        public int ReportId { get; set; }
        public string? Value { get; set; }
        public string? Content { get; set; }
        public ReportPost? ReportPost { get; set; }
        public ReportConversation? ReportConversation { get; set; }
    }

    public class ReportPost
    {
        public int? PostId { get; set; }
        public string? PostURl { get; set; }
        public string? Value { get; set; }
    }

    public class ReportConversation
    {
        public int UserId { get; set; }
        public string? Value { get; set; }
        public bool? IsGroup { get; set; }

    }

    public class PostInterestModel
    {
        public int PostId { get; set; }
        public int PostUserId { get; set; }
        public string ActionType { get; set; }
    }

}
