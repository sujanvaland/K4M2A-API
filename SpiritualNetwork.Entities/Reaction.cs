namespace SpiritualNetwork.Entities
{
    public class Reaction : BaseEntity
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Type { get; set; }
    }
}

