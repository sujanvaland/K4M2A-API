namespace SpiritualNetwork.API.Model
{
    public class ReactionReq
    {
        public string? Comment { get; set; }
        public int PostId { get; set; }
        public int? ParentId { get; set; }
    }
}
