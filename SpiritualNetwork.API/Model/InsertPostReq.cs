using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class InsertPostReq
    {
        public string PostMessage { get; set; }
        public string Type { get; set; }
    }

    public class Post
    {
        public int id { get; set; }
        public string createdBy { get; set; }
        public string userName { get; set; }
        public string createdOn { get; set; }
        public bool isPaid { get; set; }
        public string textMsg { get; set; }
        public string url { get; set; }
        public object urlMeta { get; set; }
        public List<string> imgUrl { get; set; }
        public List<string> pdfUrl { get; set; }
        public List<string> videoUrl { get; set; }
        public List<string> thumbnailUrl { get; set; }
        public int? noOfLikes { get; set; }
        public int? noOfComment { get; set; }
        public int? noOfRepost { get; set; }
        public int? noOfViews { get; set; }
        public int? isBookMarked { get; set; }
        public int? parentId { get; set; }
        public string type { get; set; }
        public string? profileImg { get; set; }
        public int? pollId { get; set; }
        public string poll { get; set; }
        public string eventDetail { get; set; }
        public List<Mentions> mentions { get;set; }
        public List<TagUser> tagUser { get; set; }
        public int? whoCanReply { get; set; }
        public int? isPinPost { get; set; }
        public string? latitude { get; set; }
        public string? longitude { get; set; }
    }
    public class PollRequest
    {
        public int id { get; set; }
        public int createdBy { get; set; }
        public int modifiedBy { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime modifiedDate { get; set; }
        public bool isDeleted { get; set; }
        public string pollTitle { get; set; }
        public string choice1 { get; set; }
        public string choice2 { get; set; }
        public string? choice3 { get; set; }
        public string? choice4 { get; set; }
        public string day { get; set; }
        public string hour { get; set; }
        public string minute { get; set; }
    }

    public class eventRequest
    {
        public int id { get; set; }
        public int createdBy { get; set; }
        public int modifiedBy { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime modifiedDate { get; set; }
        public bool isDeleted { get; set; }
        public string eventFormat { get; set; }
        public int eventTypeId { get; set; }
        public string eventTitle { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string description { get; set; }
        public string eventLink { get; set; }
        public string eventaddress { get; set; }
        public string eventCoverImage { get; set; }

    }
}
