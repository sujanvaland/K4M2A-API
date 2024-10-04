namespace SpiritualNetwork.API.Model
{
    public class UserNetworkReq
    {
        public List<UserNetwrokReqModel> list {  get; set; } 
    }

    public class UserNetwrokReqModel
    {
        public string? UniqueId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Photo { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class DeviceTokenReq
    {
        public string Token { get; set; }
		public string Type { get; set; }
	}
}
