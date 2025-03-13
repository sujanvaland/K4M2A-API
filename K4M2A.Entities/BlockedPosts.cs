using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K4M2A.Entities
{
    public class BlockedPosts : BaseEntity
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
    }
}
