using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class OnlineUsers : BaseEntity
    {
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
    }
}
