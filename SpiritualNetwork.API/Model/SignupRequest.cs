namespace SpiritualNetwork.API.Model
{
    public class SignupRequest
    {
        public string InviterName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool? IsBusinessAccount { get; set; }
        public List<AnswerModel>? Answers { get; set; }
        public string? LoginMethod { get; set; }
		public int? IsEmailVerified { get; set; } = 0;
	}

	public class ClaimUsernameRequest
    {
        public string InviterName { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
