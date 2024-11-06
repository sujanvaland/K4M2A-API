﻿using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
	public class EmailVerificationReq
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }

	}

	public class VerifiedEmail
	{
		public string OTP { get; set; }
		public string Email { get; set; }

	}

    public class PhoneVerificationReq
    {
        public long Phone { get; set; }
    }

    public class VerifiedPhone
    {
        public string OTP { get; set; }
        public long Phone { get; set; }

    }

}
