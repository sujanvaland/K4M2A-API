using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities
{
    public class PasswordResetRequest : BaseEntity
    {
        public int UserId { get; set; }
        public string OTP { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpirtionDate { get; set; }
        public bool IsUsed { get; set; }

    }

    public class EmailVerificationRequest : BaseEntity
    {
        public string? Email { get; set; }
		public string OTP { get; set; }
		public DateTime ActivationDate { get; set; }
		public DateTime ExpirtionDate { get; set; }
		public bool IsUsed { get; set; }
	}
    public class PhoneVerificationRequest : BaseEntity
    {
        public long PhoneNumber { get; set; }
        public string OTP { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpirtionDate { get; set; }
        public bool IsUsed { get; set; }
    }
}
