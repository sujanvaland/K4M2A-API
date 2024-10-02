using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities 
{
    public class Notification : BaseEntity
    {
        public int PostId { get; set; }
        public int ActionByUserId { get; set; }
        [MaxLength(20)]
        public string ActionType { get; set; }
        [MaxLength(10)]
        public string RefId1 { get; set; }
        [MaxLength(10)]
        public string RefId2 { get; set; }
        [MaxLength(100)]
        public string Message { get; set; }
        public bool IsPush { get; set; }
		public bool IsEmail { get; set; }
		public bool IsSMS { get; set; }

	}
}
