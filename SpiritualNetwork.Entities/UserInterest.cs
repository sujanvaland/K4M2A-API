using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class UserInterest : BaseEntity
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int PostUserId { get; set; }
        public string PostHashTag { get; set; }
        public string ActionType { get; set; }
    }
}
