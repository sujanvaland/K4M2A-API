using K4M2A.Entities;

namespace K4M2A.Entities.Model
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

    public class ReportBugsReq
    {
        public string? BugTitle { get; set; }
        public string? BugDescription { get; set; }
        public string? Priority { get; set; }
        public List<string>? Files { get; set; }
    }

    public class ReportBugsDataDto
    {
        public Dictionary<string, string> FormFields { get; set; } = new Dictionary<string, string>();
        public List<FileDataDto> Files { get; set; } = new List<FileDataDto>();
    }

}
