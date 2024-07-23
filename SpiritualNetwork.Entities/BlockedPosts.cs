using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class BlockedPosts : BaseEntity
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
    }
}
