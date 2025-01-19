using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class UserNotification : BaseEntity
    {
        public int NotificationId { get;set; }
        public int UserId { get;set; }
        public bool IsRead { get; set; }
		public bool IsPush { get; set; }
		public bool IsEmail { get; set; }
		public bool IsSMS { get; set; }
	}
}
