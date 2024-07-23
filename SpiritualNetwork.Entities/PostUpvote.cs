using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.Entities
{
    public class PostUpvote : BaseEntity
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        [MaxLength(10)]
        public string Type { get; set; }
    }
}
