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
        public string Phone { get; set; }
    }

    public class VerifiedPhone
    {
        public string OTP { get; set; }
        public string Phone { get; set; }

    }

	public class RequestInviteRequest
	{
		public int id { get; set; }
		public string email { get; set; }
		public string name { get; set; }
		public string phone { get; set; }
		public string city { get; set; }
		public string journey { get; set; }
		public string inviter { get; set; }
	}

}
