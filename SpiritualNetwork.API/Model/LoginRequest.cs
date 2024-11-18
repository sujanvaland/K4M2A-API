namespace SpiritualNetwork.API.Model
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
		public string? Mobile { get; set; }
	}
}
