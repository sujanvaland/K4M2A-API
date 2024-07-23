namespace SpiritualNetwork.Entities
{
    public class ChatMessages : BaseEntity
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int? GroupId { get; set; }
        public int? ReplyId { get; set; }
        public string? Message { get; set; }
        public bool? IsDelivered { get; set; }
        public bool? IsRead { get; set; }
        public int? AttachmentId { get; set; }
        public int? DeleteForUserId1 { get; set; }
        public int? DeleteForUserId2 { get; set; }
    }

    public class ChatMessagesResponse : BaseEntity
    {
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; }
        public bool? IsDelivered { get; set; }
        public bool? IsRead { get; set; }
        public int? AttachmentId { get; set; }
        public int? DeleteForUserId1 { get; set; }
        public int? DeleteForUserId2 { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ActualUrl { get; set; }
    }
}
