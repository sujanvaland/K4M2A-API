﻿namespace SpiritualNetwork.API.Model
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
		public string? Mobile { get; set; }
        public string? LoginMethod { get; set; }
	}

	public class ICOLoginRequest
	{
		public string WalletAddress { get; set; }
		public string Signature { get; set; }
		public string Message { get; set; }
	}
}
