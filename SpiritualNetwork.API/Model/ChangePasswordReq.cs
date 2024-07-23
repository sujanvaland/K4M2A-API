namespace SpiritualNetwork.API.Model
{
    public class ChangePasswordReq
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
