using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.Entities
{
    public class PostComment : BaseEntity
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }
        public int ParentId { get; set; }
    }
}
